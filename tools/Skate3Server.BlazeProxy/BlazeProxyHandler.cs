using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using NLog;
using Org.BouncyCastle.Crypto.Tls;
using Skate3Server.Blaze;

namespace Skate3Server.BlazeProxy
{
    /// <summary>
    /// Used for proxying to the real server and printing requests (for debug only)
    /// </summary>
    public class BlazeProxyHandler : ConnectionHandler
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly BlazeProxyParser _parser;
        private readonly string _proxyHost;
        private readonly int _proxyPort;
        private readonly bool _proxySecure;

        public BlazeProxyHandler(BlazeProxyParser parser, string proxyHost, int proxyPort, bool proxySecure)
        {
            _parser = parser;
            _proxyHost = proxyHost;
            _proxyPort = proxyPort;
            _proxySecure = proxySecure;
        }

        public override async Task OnConnectedAsync(ConnectionContext connection)
        {

            var proxyClient = new TcpClient();
            await proxyClient.ConnectAsync(_proxyHost, _proxyPort);

            NetworkStream ogStream = proxyClient.GetStream();

            Stream proxyStream = ogStream;

            if (_proxySecure)
            {
                var protocol = new TlsClientProtocol(proxyStream, new Org.BouncyCastle.Security.SecureRandom());
                protocol.Connect(new BlazeTlsClient());
                proxyStream = protocol.Stream;
            }

            var reader = connection.Transport.Input;
            var writer = connection.Transport.Output;

            try
            {
                while (true)
                {

                    var result = await reader.ReadAsync();
                    var requestBuffer = result.Buffer;

                    var requestCopy = GetCopyOfSequence(ref requestBuffer);

                    //parse client request
                    var requestConsumed = requestBuffer.Start;
                    var requestExamined = requestBuffer.End;

                    try
                    {
                        Logger.Debug($"Parsing Request");

                        if (_parser.TryParseRequest(ref requestBuffer, out var requestProcessedLength))
                        {
                            Logger.Debug($"Request buffer length: {requestBuffer.Length}");

                            requestConsumed = requestProcessedLength;
                            requestExamined = requestConsumed;
                        }
                        else
                        {
                            Logger.Error("Failed to parse request message");
                        }

                        //send to server
                        await proxyStream.WriteAsync(requestCopy);
                        await proxyStream.FlushAsync();
                    }
                    finally
                    {
                        reader.AdvanceTo(requestConsumed, requestExamined);
                    }

                    do
                    {
                        //receive server response
                        //Parse header
                        var headerBytes = new byte[12];
                        var headerBytesRead = await proxyStream.ReadAsync(headerBytes, 0, headerBytes.Length);
                        if (headerBytesRead != 12)
                        {
                            Logger.Error("Failed to read response header");
                        }

                        var headerSeq = new ReadOnlySequence<byte>(headerBytes);
                        Logger.Debug($"Parsing Response Header");
                        if (!_parser.TryParseResponseHeader(ref headerSeq, out var responseHeader))
                        {
                            Logger.Error("Failed to parse response header");
                        }

                        await writer.WriteAsync(headerBytes);

                        if (responseHeader.Length > 0)
                        {
                            var responseBytes = new byte[responseHeader.Length];
                            var responseBytesRead = await proxyStream.ReadAsync(responseBytes, 0, responseBytes.Length);

                            if (responseHeader.Length != responseBytesRead)
                            {
                                Logger.Error(
                                    $"Read short! Response header length: {responseHeader.Length}, read: {responseBytesRead}");
                            }

                            Logger.Debug(
                                $"Response header length: {responseHeader.Length}, buffer read: {responseBytesRead}");

                            //parse what was received from server
                            var responseSequence = new ReadOnlySequence<byte>(responseBytes);
                            Logger.Debug($"Parsing Response Body");
                            if (!_parser.TryParseResponseBody(ref responseSequence))
                            {
                                Logger.Error("Failed to parse response body");
                            }
                            await writer.WriteAsync(responseBytes);
                        }

                        //send response to client
                        await writer.FlushAsync();
                    } while (ogStream.DataAvailable);
                }
            }
            finally
            {
                await reader.CompleteAsync();
            }
        }

        private byte[] GetCopyOfSequence(ref ReadOnlySequence<byte> requestBuffer)
        {
            var array = new byte[requestBuffer.Length];
            var requestCopy = new Span<byte>(array);
            requestBuffer.CopyTo(requestCopy);
            return requestCopy.ToArray();
        }
    }
}
