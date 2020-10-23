using System.Collections.Generic;
using MediatR;
using Skate3Server.Blaze.Serializer.Attributes;

namespace Skate3Server.Blaze.Handlers.Authentication.Messages
{
    [BlazeRequest(BlazeComponent.Authentication, 0x20)]
    public class DlcRequest : IRequest<DlcResponse>
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
