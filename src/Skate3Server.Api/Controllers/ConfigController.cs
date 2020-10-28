using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace Skate3Server.Api.Controllers
{
    [Route("/skate3/config")]
    public class ConfigController : Controller
    {
        [HttpGet("PS3.xml")]
        public ActionResult Ps3()
        {
            //TODO
            var testConfig = @"<Config></Config>";

            return Content($"<?xml version=\"1.0\"?>{testConfig}", new MediaTypeHeaderValue("text/xml"));
        }
    }
}
