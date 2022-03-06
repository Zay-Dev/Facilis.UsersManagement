using Facilis.UsersManagement.SampleApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace Facilis.UsersManagement.SampleApp.Controllers
{
    [Route("~/api/user-tokens")]
    public class ApiUserTokensController : ControllerBase
    {
        private UserOtpService service { get; }

        #region Constructor(s)

        public ApiUserTokensController(UserOtpService service)
        {
            this.service = service;
        }

        #endregion Constructor(s)

        [HttpGet]
        [Route("{username}")]
        public IActionResult Generate(string username)
        {
            return new JsonResult(new
            {
                Id = this.service.GetGeneratedTokenId(username, out var token),
                Token = token
            });
        }
    }
}