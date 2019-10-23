using Mel.Live.Data;
using Mel.Live.Extensions;
using Mel.Live.Extensions.UnityExtensions;
using Mel.Live.Models.Entities;
using Mel.Live.Models.ViewModels;
using Mel.Live.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mel.Live.Controllers.BackOffice
{
    [Route("api/backOffice/[controller]")]
    [ApiController]
    [AutoBuild]
    public class AuthController : ControllerBase
    {
        #region Properties

        #region Services
        [DeepDependency]
        UserManager<User> UserManager { get; }

        [DeepDependency]
        MongoDataContext DataContext { get; }

        [DeepDependency]
        IJwtFactory JwtFactory { get; }
        #endregion

        #endregion

        #region Methods
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginViewModel model)
        {
            User user = null;

            if (model.Username.IsEmailAddress())
                user = await UserManager.FindByEmailAsync(model.Username);
            if (user == null)
                user = await UserManager.FindByNameAsync(model.Username);

            if (user == null) return NotFound();

            if (!await UserManager.CheckPasswordAsync(user, model.Password))
                return Unauthorized();

            return Ok(new AccessTokenModel() { AccessToken = await JwtFactory.GenerateToken(user) });
        }

        #endregion
    }
}
