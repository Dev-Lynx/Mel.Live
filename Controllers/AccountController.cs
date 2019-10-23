using AutoMapper;
using Mel.Live.Data;
using Mel.Live.Extensions.UnityExtensions;
using Mel.Live.Models.Entities;
using Mel.Live.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mel.Live.Controllers
{
    [AutoBuild]
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        #region Properties

        #region Services
        [DeepDependency]
        IMapper Mapper { get; }

        [DeepDependency]
        ILogger Logger { get; set; }

        [DeepDependency]
        MongoDataContext DataContext { get; set; }

        [DeepDependency]
        UserManager<User> UserManager { get; set; }
        #endregion

        #endregion

        #region Methods
        [HttpGet]
        public async Task<IActionResult> GetAccount()
        {
            User user = await UserManager.FindByIdAsync(User.FindFirst("id").Value);

            if (user == null) return Unauthorized();

            return Ok(Mapper.Map<UserViewModel>(user));
        }


        // public async Task<IActionResult> GetB
        #endregion
    }
}
