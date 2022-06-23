using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Skateboard3Server.Web.Services.Models.Common;
using Skateboard3Server.Web.Services.Models.SkateProfile;
using Skateboard3Server.Web.Storage;

namespace Skateboard3Server.Web.Services;

[Route("/skate3/ws/SkateProfile.asmx")]

[ApiController]
public class SkateProfileServiceController : ControllerBase
{
    private readonly IBlobStorage _blobStorage;

    public SkateProfileServiceController(IBlobStorage blobStorage)
    {
        _blobStorage = blobStorage;
    }

    [HttpPost("StartLoginProcess")]
    [Consumes("application/x-www-form-urlencoded")]
    [Produces("text/xml")]
    public StartLoginProcessResponse StartLoginProcess([FromForm] StartLoginProcess data)
    {
        return new StartLoginProcessResponse
        {
            PrivacyFlagContainer = string.Empty, //TODO: forces a self-closing tag
            AwardedBoardSales = 0,
            TeamInfo = new TeamInfo //TODO
            {
                TeamId = 0,
                NumMembers = 0,
            }
        };
    }

    [HttpGet("GetSchema")]
    public async Task<IActionResult> GetSchema(PlatformType platformId, uint userId) //Note this is not authed on the real version
    {
        var objectKey = $"{platformId}/{userId}";
        if (!_blobStorage.ObjectExists("user-schema", objectKey))
        {
            return NotFound();
        }
        var bytes = await _blobStorage.GetObject("user-schema", objectKey);
        if (bytes == null)
        {
            return NotFound();
        }
        return File(bytes, "application/octet-stream");
    }

    [HttpPost("UploadSchema")]
    [Consumes("multipart/form-data")]
    [Produces("text/xml")]
    public async Task<LongContainer> UploadSchema([FromForm] UploadSchema data)
    {
        if (data.Schema != null)
        {
            using (var stream = new MemoryStream()) //hack
            {
                await data.Schema.CopyToAsync(stream);
                await _blobStorage.PutObject("user-schema", $"{data.PlatformId}/{data.UserId}", stream.ToArray());
            }
            return new LongContainer(1); //TODO no idea if this number is right
        }
        return new LongContainer(0); //TODO no idea if this number is right
    }

    [HttpPost("UploadAIProfile")]
    [Consumes("multipart/form-data")]
    [Produces("text/xml")]
    public LongContainer UploadSchema([FromForm] UploadAiProfile data)
    {
        //TODO save AI profile?
        return new LongContainer(2); //TODO no idea if this number is right
    }

    [HttpPost("SetUserDLC")]
    [Consumes("application/x-www-form-urlencoded")]
    [Produces("text/xml")]
    public BoolContainer SetUserDlc([FromForm] SetUserDlc data)
    {
        return new BoolContainer(true);
    }

    [HttpPost("SetUserAchievements")]
    [Consumes("application/x-www-form-urlencoded")]
    [Produces("text/xml")]
    public IntegerContainer SetUserAchievements([FromForm] SetUserAchievements data)
    {
        return new IntegerContainer(0);
    }
}