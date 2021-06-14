using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Skateboard3Server.Blaze.Managers;
using Skateboard3Server.Data;

namespace Skateboard3Server.Web.Controllers
{
    /// <summary>
    /// These are the entrypoints for the web browser from here auth is done, cookies set, and redirected
    /// </summary>
    [Route("/skate3/webkit/PS3/English/i/Sessions")]
    public class SessionsController : ControllerBase
    {
        private readonly Skateboard3Context _dbContext;
        private readonly IUserSessionManager _userSessionManager;

        public SessionsController(Skateboard3Context dbContext, IUserSessionManager userSessionManager)
        {
            _dbContext = dbContext;
            _userSessionManager = userSessionManager; //TODO handle usersessionmanager depend better
        }

        [HttpGet("GameLogin/{sessionKey}/bootflow")]
        public async Task<IActionResult> BootFlow(string sessionKey)
        {
            if (!await AuthenticateUser(sessionKey))
            {
                return Redirect("/skate3/webkit/ErrorPages/404");
            }

            return Redirect("/skate3/webkit/PS3/English/i/SkateFeed");
        }

        [HttpGet("GameLogin/{sessionKey}/skatefeed")]
        public async Task<IActionResult> SkateFeed(string sessionKey)
        {
            if (!await AuthenticateUser(sessionKey))
            {
                return Redirect("/skate3/webkit/ErrorPages/404");
            }

            return Redirect($"/skate3/webkit/PS3/English/i/SkateFeed");
        }

        [HttpGet("GameLogin/{sessionKey}/playerprofile")]
        public async Task<IActionResult> PlayerProfile(string sessionKey)
        {
            if (!await AuthenticateUser(sessionKey))
            {
                return Redirect("/skate3/webkit/PS3/ErrorPages/404");
            }

            return Redirect($"/skate3/webkit/PS3/English/i/Users/Show/{HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)}");
        }

        [HttpGet("GameLogin/{sessionKey}/playerprofile{profileId}")]
        public async Task<IActionResult> OtherPlayerProfile(string sessionKey, string profileId)
        {
            //Used from recent players/friends view profile
            if (!await AuthenticateUser(sessionKey))
            {
                return Redirect("/skate3/webkit/PS3/ErrorPages/404");
            }
            //TODO check if profile exists
            if(!uint.TryParse(profileId, out uint parsedProfileId))
            {
                return Redirect("/skate3/webkit/PS3/ErrorPages/404");
            }

            return Redirect(
                $"/skate3/webkit/PS3/English/i/Users/Show/{parsedProfileId}");
        }

        [HttpGet("GameLogin/{sessionKey}/leaderboards")]
        public async Task<IActionResult> Leaderboards(string sessionKey)
        {
            if (!await AuthenticateUser(sessionKey))
            {
                return Redirect("/skate3/webkit/PS3/ErrorPages/404");
            }

            return Redirect($"/skate3/webkit/PS3/English/i/Leaderboards");
        }

        [HttpGet("GameLogin/{sessionKey}/createteam")]
        public async Task<IActionResult> CreateTeam(string sessionKey)
        {
            if (!await AuthenticateUser(sessionKey))
            {
                return Redirect("/skate3/webkit/PS3/ErrorPages/404");
            }

            return Redirect($"/skate3/webkit/PS3/English/i/Teams/CreateTeam");
        }

        [HttpGet("GameLogin/{sessionKey}/teamfinder")]
        public async Task<IActionResult> TeamFinder(string sessionKey)
        {
            if (!await AuthenticateUser(sessionKey))
            {
                return Redirect("/skate3/webkit/PS3/ErrorPages/404");
            }

            return Redirect($"/skate3/webkit/PS3/English/i/Teams");
        }

        [HttpGet("GameLogin/{sessionKey}/myinvites")]
        public async Task<IActionResult> MyInvites(string sessionKey)
        {
            if (!await AuthenticateUser(sessionKey))
            {
                return Redirect("/skate3/webkit/PS3/ErrorPages/404");
            }

            return Redirect($"/skate3/webkit/PS3/English/i/Users/Invitations/{HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)}");
        }

        private async Task<bool> AuthenticateUser(string sessionKey)
        {
            var userSessionData = _userSessionManager.GetUserSessionDataForKey(sessionKey);
            if (userSessionData == null)
            {
                return false;
            }

            var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Id == userSessionData.UserId);

            if (user == null)
            {
                return false;
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,  Convert.ToString(user.Id)),
                new Claim(ClaimTypes.Name,  user.Username),
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                //AllowRefresh = <bool>,
                // Refreshing the authentication session should be allowed.

                //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                // The time at which the authentication ticket expires. A 
                // value set here overrides the ExpireTimeSpan option of 
                // CookieAuthenticationOptions set with AddCookie.

                //IsPersistent = true,
                // Whether the authentication session is persisted across 
                // multiple requests. When used with cookies, controls
                // whether the cookie's lifetime is absolute (matching the
                // lifetime of the authentication ticket) or session-based.

                //IssuedUtc = <DateTimeOffset>,
                // The time at which the authentication ticket was issued.

                //RedirectUri = <string>
                // The full path or absolute URI to be used as an http 
                // redirect response value.
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return true;

        }
    }
}
