using System;
using System.Buffers;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace SkateServer.Blaze.Components
{
    public class RedirectorComponent : IBlazeComponent
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public void HandleRequest(BlazeRequest request, Stream output)
        {
            switch (request.Command)
            {
                case 1:
                    SendServerInstance(request, output);
                    break;
                default:
                    Logger.Error($"Unknown Redirector command: {request.Command}");
                    break;

            }
            
        }

        public void SendServerInstance(BlazeRequest request, Stream output)
        {
            //We dont care about the request for now just reply with what we know

            var host = Encoding.UTF8.GetBytes("localhost");
            var ip = IPAddress.Parse("127.0.0.1");
            var port = 10744;
            byte secure = 0x0; //should be 1 but we are testing

            //TODO this is gross
            //Body
            var bodyStream = new MemoryStream();
            TdfHelper.WriteLabel(bodyStream, "ADDR");
            TdfHelper.WriteTypeAndLength(bodyStream, TdfType.Union, 0);
            bodyStream.WriteByte(0x0); //network address type
            TdfHelper.WriteLabel(bodyStream, "VALU");

            bodyStream.WriteByte(0x0); //struct start
            TdfHelper.WriteLabel(bodyStream, "HOST");
            TdfHelper.WriteTypeAndLength(bodyStream, TdfType.String, Convert.ToUInt32(host.Length + 1));
            bodyStream.Write(host);
            bodyStream.WriteByte(0x0); //terminate string
            TdfHelper.WriteLabel(bodyStream, "IP");
            TdfHelper.WriteTypeAndLength(bodyStream, TdfType.Uint32, 0x4);
            var addressBytes = BitConverter.GetBytes(Convert.ToUInt32(ip.Address));
            bodyStream.Write(addressBytes); //not big endian
            TdfHelper.WriteLabel(bodyStream, "PORT");
            TdfHelper.WriteTypeAndLength(bodyStream, TdfType.Uint16, 0x2);
            var portBytes = BitConverter.GetBytes(Convert.ToUInt16(port));
            Array.Reverse(portBytes); //big endian
            bodyStream.Write(portBytes);
            bodyStream.WriteByte(0x0); //struct end

            TdfHelper.WriteLabel(bodyStream, "SECU");
            TdfHelper.WriteTypeAndLength(bodyStream, TdfType.Int8, 0x1);
            bodyStream.WriteByte(secure);
            TdfHelper.WriteLabel(bodyStream, "XDNS");
            TdfHelper.WriteTypeAndLength(bodyStream, TdfType.Uint32, 0x4);
            bodyStream.Write(BitConverter.GetBytes(Convert.ToUInt32(0)));

            //TODO: oh god no (big endian conversion)
            var fullStream = new MemoryStream();

            //Header
            //Length
            var length = BitConverter.GetBytes(Convert.ToUInt16(bodyStream.Length));
            Array.Reverse(length);//big endian
            fullStream.Write(length);
            //Component
            var component = BitConverter.GetBytes(Convert.ToUInt16(BlazeComponent.Redirector));
            Array.Reverse(component);//big endian
            fullStream.Write(component);
            //Command
            var command = BitConverter.GetBytes(Convert.ToUInt16(1));
            Array.Reverse(command);//big endian
            fullStream.Write(command);
            //ErrorCode
            var errorCode = BitConverter.GetBytes(Convert.ToUInt16(0));
            Array.Reverse(errorCode);//big endian
            fullStream.Write(errorCode);
            //MessageType/MessageId
            var messageData =
                BitConverter.GetBytes(Convert.ToUInt32((int) BlazeMessageType.Reply << 28 | request.MessageId));
            Array.Reverse(messageData);//big endian
            fullStream.Write(messageData);
            //Body
            fullStream.Write(bodyStream.ToArray());

            //gross but for debugging
            output.Write(fullStream.ToArray());

            //var payloadHex = BitConverter.ToString(fullStream.ToArray()).Replace("-", " ");
            //Logger.Debug(payloadHex);
        }
    }
}
