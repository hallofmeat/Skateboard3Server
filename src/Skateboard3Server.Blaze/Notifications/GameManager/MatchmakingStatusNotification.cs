using System.Collections.Generic;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Notifications.GameManager
{
    [BlazeNotification(BlazeComponent.GameManager, (ushort)GameManagerNotification.MatchmakingStatus)]
    public class MatchmakingStatusNotification : BlazeNotification
    {
        [TdfField("ASIL")]
        public List<AsilData> Asil { get; set; } //TODO

        [TdfField("MSID")]
        public uint MatchmakingSessionId { get; set; }

        [TdfField("USID")]
        public uint UserSessionId { get; set; }
    }

    public class AsilData //TODO: name
    {
        [TdfField("CGS")]
        public CgsData Cgs { get; set; } //TODO

        [TdfField("CUST")]
        public CustData Cust { get; set; } //TODO

        [TdfField("DNFS")]
        public DnfsData Dnfs { get; set; } //TODO

        [TdfField("FGS")]
        public FgsData Fgs { get; set; } //TODO

        [TdfField("GEOS")]
        public GeosData Geos { get; set; } //TODO

        [TdfField("GRDA")]
        public Dictionary<string, GrdaData> Grda { get; set; } //TODO

        [TdfField("GSRD")]
        public GsrdData Gsrd { get; set; } //TODO game slot data?

        [TdfField("HBRD")]
        public HbrdData Hbrd { get; set; } //TODO

        [TdfField("HVRD")]
        public HvrdData Hvrd { get; set; } //TODO

        [TdfField("PSRS")]
        public PingServerNames PingServerNames { get; set; }

        [TdfField("RRDA")]
        public RrdaData Rrda { get; set; } //TODO

        [TdfField("TSRS")]
        public TsrsData Tsrs { get; set; } //TODO
    }


    public class CgsData //TODO: name
    {
        [TdfField("EVST")]
        public uint Evst { get; set; } //TODO

        [TdfField("MMSN")]
        public uint Mmsn { get; set; } //TODO

        [TdfField("NOMP")]
        public uint Nomp { get; set; } //TODO
    }

    public class CustData //TODO: name
    {
        [TdfField("EXPS")]
        public ExpsData Exps { get; set; } //TODO
    }

    public class ExpsData //TODO: name
    {
        [TdfField("MASK")]
        public ulong Mask { get; set; } //TODO DLC mask?
    }

    public class DnfsData //TODO: name
    {
        [TdfField("MDNF")]
        public int Mdnf { get; set; } //TODO: enum

        [TdfField("XDNF")]
        public int Xdnf { get; set; } //TODO: enum
    }

    public class FgsData //TODO: name
    {
        [TdfField("GNUM")]
        public uint Gnum { get; set; } //TODO
    }

    public class GeosData
    {
        [TdfField("DIST")]
        public uint Dist { get; set; } //TODO
    }

    public class GrdaData //TODO: name
    {
        [TdfField("NAME")]
        public string Name { get; set; }

        [TdfField("VALU")]
        public List<string> Values { get; set; }
    }

    public class GsrdData //TODO: name
    {
        [TdfField("PMAX")]
        public uint Pmax { get; set; } //TODO Player Max?

        [TdfField("PMIN")]
        public uint Pmin { get; set; } //TODO Player Min?
    }

    public class HbrdData //TODO: name
    {
        [TdfField("BVAL")]
        public int Bval { get; set; } //TODO: enum
    }

    public class HvrdData //TODO: name
    {
        [TdfField("VVAL")]
        public int Vval { get; set; } //TODO: enum
    }

    public class PingServerNames
    {
        [TdfField("VALU")]
        public List<string> Values { get; set; }
    }

    public class RrdaData //TODO: name
    {
        [TdfField("RVAL")]
        public byte Rval { get; set; }
    }

    public class TsrsData //TODO: name
    {
    }

}
