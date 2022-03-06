using Facilis.Core.Abstractions;
using Facilis.UsersManagement.Abstractions;
using Facilis.UsersManagement.Helpers;
using Facilis.UsersManagement.SampleApp.Enums;
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
        private IAuthenticator<IPasswordBase, User> passwordAuthenticator { get; }
        private IEntities<User> users { get; }

        #region Constructor(s)

        public HomeController(
            IAuthenticator<IPasswordBase, User> passwordAuthenticator,
            IEntities<User> users
        )
        {
            this.passwordAuthenticator = passwordAuthenticator;
            this.users = users;
        }

        #endregion Constructor(s)

        public IActionResult Index()
        {
            var userId = this.User.GetIdentifier()?.Value;
            return View(this.users.FindById(userId));
        }

        [Route("~/users")]
        [Route("~/users/{id}")]
        public IActionResult EditUser(string id)
        {
            if (!this.User.IsInRole(RoleTypes.Administrator))
            {
                return Unauthorized();
            }

            if (string.IsNullOrEmpty(id)) return View();

            var user = this.users.FindById(id);
            return user == null ? NotFound() : View(user);
        }

        [AllowAnonymous]
        [Route("~/sign-in")]
        public IActionResult SignIn()
        {
            return View();
        }

        [AllowAnonymous]
        [Route("~/sign-in/otp")]
        public IActionResult SignInWithToken()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("~/sign-in/otp")]
        public async Task<IActionResult> SignInWithToken(
            [FromServices] IAuthenticator<ITokenBase, User> authenticator,
            string tokenId,
            string value
        )
        {
            var authenticated = authenticator
                .TryAuthenticate(new TokenBase()
                {
                    TokenId = tokenId,
                    Value = value ?? ""
                });

            return await GetActionResultAsync("", authenticated);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("~/sign-in")]
        public async Task<IActionResult> SignIn(string username, string password)
        {
            var authenticated = this.passwordAuthenticator
                .TryAuthenticate(new PasswordBase()
                {
                    Username = username,
                    Password = password,
                });

            return await GetActionResultAsync(username, authenticated);
        }

        [Route("~/sign-out")]
        public async Task<IActionResult> SignOutAsync()
        {
            await this.HttpContext.SignOutAsync();
            return Redirect("~/");
        }

        private async Task<IActionResult> GetActionResultAsync(string username, IAuthenticatedResult<User> authenticated)
        {
            if (authenticated.HasFailure())
            {
                this.SaveFailureTempData(username);
                return RedirectToAction(nameof(SignIn));
            }

            await this.SignInAsync(authenticated.User);
            return Redirect("~/");
        }

        private async Task SignInAsync(User user)
        {
            var profile = user.Profile;
            var identity = new ClaimsIdentity(
                user.ToClaims(),
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            await this.HttpContext.SignInAsync(new ClaimsPrincipal(identity));

            profile.LastSignInAtUtc = DateTime.UtcNow;
            user.SetProfile(profile);
            this.users.Update(user);
        }

        private void SaveFailureTempData(string username)
        {
            TempData["Username"] = username;
            TempData["IsRetry"] = true;
        }
    }
}