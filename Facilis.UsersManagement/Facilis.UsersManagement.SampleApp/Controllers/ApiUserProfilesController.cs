using Facilis.Core.Abstractions;
using Facilis.UsersManagement.Abstractions;
using Facilis.UsersManagement.Models;
using Facilis.UsersManagement.SampleApp.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Facilis.UsersManagement.SampleApp.Controllers
{
    [Authorize]
    [Route("~/api/users/{userId}/profiles")]
    public class ApiUserProfilesController : ControllerBase
    {
        private IEntities<User> users { get; }

        #region Constructor(s)

        public ApiUserProfilesController(IEntities<User> users)
        {
            this.users = users;
        }

        #endregion Constructor(s)

        [Route("~/api/user-profiles")]
        public IActionResult ListUsers()
        {
            if (!this.IsMeAdmin()) return Unauthorized();

            return new JsonResult(this.users
                .WhereEnabled()
                .Select(user => new { user, profile = user.Profile as UserProfile })
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

        [HttpPatch]
        public IActionResult Index(string userId, [FromBody] UserProfile model)
        {
            var isAdmin = this.IsMeAdmin();
            var myUserId = this.User
                .Claims
                .First(claim => claim.Type == ClaimTypes.NameIdentifier)
                .Value;

            if (!isAdmin && userId != myUserId) return BadRequest();

            var user = this.users.FindById(userId);
            if (user?.Profile is not UserProfile profile)
            {
                return NotFound();
            }

            if (isAdmin) profile.Email = model.Email;
            profile.Nickname = model.Nickname;
            profile.FirstName = model.FirstName;
            profile.LastName = model.LastName;

            user.SetProfile(profile);
            this.users.Update(user);
            return Ok();
        }

        private bool IsMeAdmin()
        {
            return this.User.IsInRole(nameof(RoleTypes.Administrator));
        }
    }
}