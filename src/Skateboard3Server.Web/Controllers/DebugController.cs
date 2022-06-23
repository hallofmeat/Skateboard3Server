using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace Skateboard3Server.Web.Controllers;

/// <summary>
/// Not a real controller used by skate, used by admin or other services
/// </summary>
[Route("/debug")]
[ApiController]
public class DebugController : ControllerBase
{
    [HttpGet("status")]
    public StatusUpdate Status()
    {
        var current = System.Diagnostics.Process.GetCurrentProcess();
        return new StatusUpdate
        {
            StartTime = current.StartTime
        };
    }
}

public class StatusUpdate //Keep in sync with Skateboard3Server.Qos
{
    //TODO: maybe connected users?
    public DateTime StartTime { get; set; } 
}