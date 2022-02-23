using Facilis.Core.Abstractions;
using Facilis.UsersManagement.Abstractions;
using Facilis.UsersManagement.Enums;
using Facilis.UsersManagement.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Facilis.UsersManagement.SampleApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private IAuthenticator authenticator { get; }
        private IEntities<User<UserProfile>> users { get; }

        #region Constructor(s)

        public HomeController(
            IAuthenticator authenticator,
            IEntities<User<UserProfile>> users
        )
        {
            this.authenticator = authenticator;
            this.users = users;
        }

        #endregion Constructor(s)

        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [Route("~/sign-in")]
        public IActionResult SignIn()
        {
            return View();
        }

        [AllowAnonymous]
        [Route("~/sign-in")]
        [HttpPost]
        public async Task<IActionResult> SignIn(string username, string password)
        {
            var failureType = this.authenticator
                .TryAuthenticate(username, password, out var user);

            if (failureType != LoginFailureTypes.None)
            {
                TempData["Username"] = username;
                TempData["IsRetry"] = true;
                return RedirectToAction(nameof(SignIn));
            }

            var profile = (UserProfile)user.Profile;
            var roleClaims = profile.Roles
                .Select(role => new Claim(ClaimTypes.Role, role))
                .ToArray();
            var identity = new ClaimsIdentity(
                roleClaims.Concat(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.Username),
                }),
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            await this.HttpContext.SignInAsync(new ClaimsPrincipal(identity));

            profile.LastSignInAtUtc = DateTime.UtcNow;
            user.SetProfile(profile);
            this.users.Update((User<UserProfile>)user);

            return Redirect("~/");
        }

        [Route("~/sign-out")]
        public async Task<IActionResult> SignOut()
        {
            await this.HttpContext.SignOutAsync();
            return Redirect("~/");
        }
    }
}