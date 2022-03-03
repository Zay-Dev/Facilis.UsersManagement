using Facilis.Core.Abstractions;
using Facilis.UsersManagement.Abstractions;
using Facilis.UsersManagement.Helpers;
using Facilis.UsersManagement.Models;
using Facilis.UsersManagement.SampleApp.Enums;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Facilis.UsersManagement.SampleApp.Controllers
{
    [Authorize]
    [Route("~/api/users/{userId}/profiles")]
    public class ApiUserProfilesController : ControllerBase
    {
        private IEntityStampsBinder entityStampsBinder { get; }
        private IEntities<User> users { get; }

        #region Constructor(s)

        public ApiUserProfilesController(
            IEntityStampsBinder entityStampsBinder,
            IEntities<User> users
        )
        {
            this.entityStampsBinder = entityStampsBinder;
            this.users = users;
        }

        #endregion Constructor(s)

        [Route("~/api/user-profiles")]
        public IActionResult ListUsers()
        {
            if (!this.IsMeAdmin()) return Unauthorized();

            return new JsonResult(this.users
                .WhereEnabled()
                .Select(user => new { user, profile = user.Profile })
                .Select(entity => new
                {
                    entity.user.Id,
                    entity.user.Username,
                    entity.profile.Email,
                    entity.profile.Nickname,
                    entity.profile.FirstName,
                    entity.profile.LastName,
                })
            );
        }

        [HttpPost]
        [Route("~/api/users/{username}")]
        public IActionResult Index(
            [FromServices] IPasswordHasher hasher,
            [FromRoute] string username,
            [FromQuery] string password,
            [FromBody] UserProfile model
        )
        {
            if (!this.IsMeAdmin()) return BadRequest();

            var hashed = hasher.Hash(password);
            var user = new User()
            {
                Username = username,
                HashingMethod = hashed.HashingMethod,
                HashedPassword = hashed.HashedPassword,
                PasswordSalt = hashed.PasswordSalt,
                PasswordIterated = hashed.PasswordIterated,
            };

            model.Roles = new[] { nameof(RoleTypes.User) };
            model.LastSignInAtUtc = DateTime.UtcNow;
            user.SetProfile(model);

            this.entityStampsBinder.BindCreatedByUser(user);
            this.users.Add(user);

            return Ok();
        }

        [HttpPatch]
        public async Task<IActionResult> Index(string userId, [FromBody] UserProfile model)
        {
            var isAdmin = this.IsMeAdmin();
            var myUserId = this.User.GetIdentifier().Value;

            if (!isAdmin && userId != myUserId) return BadRequest();

            var user = this.users.FindById(userId);
            if (user?.Profile == null) return NotFound();

            if (isAdmin) user.Profile.Email = model.Email;
            user.Profile.Nickname = model.Nickname;
            user.Profile.FirstName = model.FirstName;
            user.Profile.LastName = model.LastName;

            user.SetProfile(user.Profile);
            this.entityStampsBinder.BindUpdatedByUser(user);

            this.users.Update(user);

            if (userId == myUserId)
            {
                await this.SignInAsync(user);
            }
            return Ok();
        }

        private async Task SignInAsync(User user)
        {
            var identity = new ClaimsIdentity(
                user.ToClaims(),
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            await this.HttpContext.SignInAsync(new ClaimsPrincipal(identity));
        }

        private bool IsMeAdmin() => this.User.IsInRole(RoleTypes.Administrator);
    }
}