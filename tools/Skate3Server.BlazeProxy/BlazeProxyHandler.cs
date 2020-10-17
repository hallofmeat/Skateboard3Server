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

        private readonly BlazeDebugParser _parser;
        private readonly string _proxyHost;
        private readonly int _proxyPort;
        private readonly bool _proxySecure;

        public BlazeProxyHandler(BlazeDebugParser parser, string proxyHost, int proxyPort, bool proxySecure)
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

                        if (_parser.TryParse(ref requestBuffer, out var requestProcessedLength))
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
                        var responseBytes = new byte[8192];
                        var bytesRead = await proxyStream.ReadAsync(responseBytes, 0, responseBytes.Length);

                        if (bytesRead == 12)
                        {
                            //notification (keep reading)
                            var notificationBytes =
                                await proxyStream.ReadAsync(responseBytes, 12, responseBytes.Length);
                            Logger.Debug($"Response buffer extra read: {notificationBytes}");
                            bytesRead += notificationBytes;
                        }


                        Logger.Debug($"Response buffer read: {bytesRead}");
                        Array.Resize(ref responseBytes, bytesRead);

                        //parse what was received from server
                        var responseSequence = new ReadOnlySequence<byte>(responseBytes);

                        Logger.Debug($"Parsing Response");

                        if (_parser.TryParse(ref responseSequence, out var responseProcessedLength))
                        {
                            Logger.Debug(
                                $"Response buffer length: {responseBytes.Length}, processed: {responseProcessedLength.GetInteger()}");
                        }
                        else
                        {
                            Logger.Error("Failed to parse response message");
                        }

                        //send response to client
                        await writer.WriteAsync(responseBytes);
                        await writer.FlushAsync();
                    } while (ogStream.DataAvailable);

                    //TODO: handle connection hangup
                    //TODO: advance reader?
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
