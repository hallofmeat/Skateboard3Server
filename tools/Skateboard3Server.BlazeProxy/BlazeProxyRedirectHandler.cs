using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Bedrock.Framework.Protocols;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Options;
using NLog;
using Skateboard3Server.Blaze;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.BlazeProxy
{
    /// <summary>
    /// Used to reply with the correct response to the redirector request
    /// </summary>
    public class BlazeProxyRedirectHandler : ConnectionHandler
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly BlazeProxySettings _proxySettings;

        public BlazeProxyRedirectHandler(IOptions<BlazeProxySettings> proxySettings)
        {
            _proxySettings = proxySettings.Value;
        }

        public override async Task OnConnectedAsync(ConnectionContext connection)
        {
            Logger.Debug($"Redirecting to {_proxySettings.RedirectHost} {_proxySettings.RedirectIp}:{_proxySettings.LocalPort}");

            var blazeProtocol = new BlazeProxyProtocol();
            var localReader = connection.CreateReader();
            var localWriter = connection.CreateWriter();

            while (true)
            {
                try
                {
                    var result = await localReader.ReadAsync(blazeProtocol);
                    var message = result.Message;

                    if (message != null)
                    {
                        if (message.Header.Component == BlazeComponent.Redirector &&
                            message.Header.Command == (ushort) RedirectorCommand.ServerInfo)
                        {
                            var hostBytes = new List<byte>(Encoding.ASCII.GetBytes(_proxySettings.RedirectHost));
                            var payload = new List<byte>();
                            payload.AddRange(new byte[] {
                                0x86, 0x49, 0x32, //ADDR
                                0xD0, 0x00, //union(0)
                                0xDA, 0x1B, 0x35, //VALU
                                0x00, //start struct
                                0xA2, 0xFC, 0xF4, //HOST
                            });
                            payload.AddRange(GetStringLengthBytes((uint)hostBytes.Count + 1));
                            payload.AddRange(hostBytes);
                            payload.AddRange(new byte[] {
                                0x00, //endbyte for string
                                0xA7, 0x00, 0x00, //IP
                                0x74, //uint32
                            });
                            var ipBytes = BitConverter.GetBytes(Convert.ToUInt32(IPAddress.Parse(_proxySettings.RedirectIp).Address));
                            Array.Reverse(ipBytes); //big endian
                            payload.AddRange(ipBytes);
                            payload.AddRange(new byte[] {
                                0xC2, 0xFC, 0xB4, //PORT
                                0x52, //uint16
                            });
                            var portBytes = BitConverter.GetBytes(Convert.ToUInt16(_proxySettings.LocalPort));
                            Array.Reverse(portBytes); //big endian
                            payload.AddRange(portBytes);
                            payload.AddRange(new byte[] {
                                0x00, //end struct
                                0xCE, 0x58, 0xF5, //SECU
                                0x21, //int8
                                0x00, //0
                                0xE2, 0x4B, 0xB3, //XDNS
                                0x74, //uint32
                                0x00, 0x00, 0x00, 0x00, //0
                            });
                            var response = new BlazeMessageData
                            {
                                Header = new BlazeHeader
                                {
                                    Command = message.Header.Command,
                                    Component = message.Header.Component,
                                    MessageType = BlazeMessageType.Reply,
                                    MessageId = message.Header.MessageId,
                                    ErrorCode = 0
                                },
                                Payload = new ReadOnlySequence<byte>(payload.ToArray())
                            };
                            await localWriter.WriteAsync(blazeProtocol, response);
                        }
                    }

                    if (result.IsCompleted)
                    {
                        break;
                    }
                }
                finally
                {
                    localReader.Advance();
                }
            }

        }

        private byte[] GetStringLengthBytes(uint length)
        {
            var output = new List<byte>();
            uint stringType = 0x1;
            //length is 15 or more
            if (length >= 0xF)
            {
                output.Add((byte)((stringType << 4) | 0xF));
                output.AddRange(GetLength(length));
            }
            else
            {
                output.Add((byte)((stringType << 4) | length));
            }
            return output.ToArray();
        }

        private byte[] GetLength(uint length)
        {
            //Length is a https://en.wikipedia.org/wiki/Variable-length_quantity
            var output = new List<byte>();

            if (length > Math.Pow(2, 56))
                throw new OverflowException("Could not write variable-length, exceeds max value");

            var index = 3;
            var significantBitReached = false;
            var mask = 0x7fUL << (index * 7);
            while (index >= 0)
            {
                var buffer = (mask & length);
                if (buffer > 0 || significantBitReached)
                {
                    significantBitReached = true;
                    buffer >>= index * 7;
                    if (index > 0)
                        buffer |= 0x80;
                    output.Add((byte)buffer);
                }
                mask >>= 7;
                index--;
            }

            if (!significantBitReached)
                output.Add(new byte());

            return output.ToArray();
        }

    }
}
