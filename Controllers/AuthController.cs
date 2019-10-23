using AutoMapper;
using Mel.Live.Data;
using Mel.Live.Extensions;
using Mel.Live.Extensions.UnityExtensions;
using Mel.Live.Models.Entities;
using Mel.Live.Models.ViewModels;
using Mel.Live.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace Mel.Live.Controllers
{
    [AutoBuild]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        #region Properties

        #region Services
        [DeepDependency]
        IMapper Mapper { get; }

        [DeepDependency]
        JwtFactory JwtFactory { get; }

        [DeepDependency]
        UserManager<User> UserManager { get; }

        [DeepDependency]
        MongoDataContext DataContext { get; }

        [DeepDependency]
        ILogger Logger { get; }
        #endregion

        #endregion

        #region Methods

        #region Reception
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterViewModel model)
        {
            User user = Mapper.Map<User>(model);

            IdentityResult result = await UserManager.CreateAsync(user);
            if (!result.Succeeded) return new BadRequestObjectResult(result.Errors);

            result = await UserManager.AddToRoleAsync(user, UserRoles.Regular.ToString());
            if (!result.Succeeded) return new BadRequestObjectResult(result.Errors);

            result = await UserManager.AddPasswordAsync(user, model.Password);
            if (!result.Succeeded) return new BadRequestObjectResult(result.Errors);

            await user.InitializeAsync();
            
            return Ok(new AccessTokenModel() {
                AccessToken = await JwtFactory.GenerateToken(user) });
        }

        /*
        [Authorize]
        public async Task<IActionResult> CreateNode()
        {

        }
        */

        /// <summary>
        /// Sign in a user
        /// </summary>
        /// <param name="model">User login details.</param>
        /// <returns>A JWT access token or a collection of the errors found.</returns>
        [HttpPost("login")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(AccessTokenModel), Description = "A JWT Access Token for user account access.")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(NotFoundResult), Description = "User does not exist on this platform.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, typeof(UnauthorizedResult), Description = "Invalid User Credentials.")]
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

        #region Password Management
        /// <summary>
        /// Change account password
        /// </summary>
        /// <param name="model">Password change model</param>
        /// <returns>An OK Response</returns>
        [Authorize]
        [HttpPost("password/reset")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(OkResult), Description = "Password was successfully changed")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, typeof(UnauthorizedResult), Description = "User was not found or the user credentials were invalid.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(IEnumerable<IdentityError>), Description = "The input was invalid. Returns a collection of the errors found.")]
        public async Task<IActionResult> ChangePassword([FromBody]PasswordUpdateViewModel model)
        {
            User user = await UserManager.GetUserAsync(User);

            if (user == null) return Unauthorized();

            IdentityResult result = await UserManager.ChangePasswordAsync(user, 
                model.CurrentPassword, model.NewPassword);

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok();
        }
        #endregion


        #endregion
    }
}
