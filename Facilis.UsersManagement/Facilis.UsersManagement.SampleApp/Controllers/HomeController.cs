﻿using Facilis.Core.Abstractions;
using Facilis.UsersManagement.Abstractions;
using Facilis.UsersManagement.Enums;
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
        private IAuthenticator<User> authenticator { get; }
        private IEntities<User> users { get; }

        #region Constructor(s)

        public HomeController(
            IAuthenticator<User> authenticator,
            IEntities<User> users
        )
        {
            this.authenticator = authenticator;
            this.users = users;
        }

        #endregion Constructor(s)

        public IActionResult Index()
        {
            var userId = this.User.GetIdentifier()?.Value;
            return View(this.users.FindById(userId));
        }

        [Route("~/users/{id}")]
        public IActionResult EditUser(string id)
        {
            if (!this.User.IsInRole(RoleTypes.Administrator))
            {
                return Unauthorized();
            }

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

            var profile = user.Profile;
            var identity = new ClaimsIdentity(
                user.ToClaims(),
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            await this.HttpContext.SignInAsync(new ClaimsPrincipal(identity));

            profile.LastSignInAtUtc = DateTime.UtcNow;
            user.SetProfile(profile);
            this.users.Update(user);

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