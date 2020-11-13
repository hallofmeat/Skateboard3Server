using System.Collections.Generic;
using MediatR;
using Skate3Server.Blaze.Serializer.Attributes;
using Skate3Server.Blaze.Server;

namespace Skate3Server.Blaze.Handlers.Authentication.Messages
{
    [BlazeRequest(BlazeComponent.Authentication, (ushort)AuthenticationCommand.Dlc)]
    public class DlcRequest : IRequest<DlcResponse>, IBlazeRequest
    {
        [TdfField("BUID")]
        public uint Buid { get; set; } //TODO

        [TdfField("EPSN")]
        public ushort Epsn { get; set; } //TODO

        [TdfField("EPSZ")]
        public ushort Epsz { get; set; } //TODO

        [TdfField("GNLS")]
        public List<string> Gnls { get; set; } //TODO

        [TdfField("OLD")]
        public bool Old { get; set; } //TODO

        [TdfField("ONLY")]
        public bool Only { get; set; } //TODO

        [TdfField("PERS")]
        public bool Pers { get; set; } //TODO
    }
}
