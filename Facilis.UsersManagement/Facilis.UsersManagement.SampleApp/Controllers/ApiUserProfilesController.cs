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

        [HttpPatch]
        public IActionResult Index(string userId, [FromBody] UserProfile model)
        {
            var isAdmin = this.User.IsInRole(nameof(RoleTypes.Administrator));
            var user = this.users.FindById(this.User
                .Claims
                .First(claim => claim.Type == ClaimTypes.NameIdentifier)
                .Value
            );

            if (user == null) return NotFound();
            if (!isAdmin && userId != user.Id) return BadRequest();

            var profile = this.users
                .FindById(userId)
                ?.Profile as UserProfile;
            if (profile == null) return NotFound();

            if (isAdmin) profile.Email = model.Email;
            profile.Nickname = model.Nickname;
            profile.FirstName = model.FirstName;
            profile.LastName = model.LastName;

            user.SetProfile(profile);
            this.users.Update(user);
            return Ok();
        }
    }
}