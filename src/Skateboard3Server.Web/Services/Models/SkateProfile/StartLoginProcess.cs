using Skateboard3Server.Web.Services.Models.Common;

namespace Skateboard3Server.Web.Services.Models.SkateProfile;

public class StartLoginProcess
{
    public PlatformType PlatformType { get; set; }
    public uint UserId { get; set; }
    public string SessionKey { get; set; }
    public bool DeleteThumbs { get; set; }
    public int Difficulty { get; set; } //TODO: enum?
    public ulong TotalBoardSales { get; set; }
}