using AutoMapper;
using Mel.Live.Data;
using Mel.Live.Extensions.UnityExtensions;
using Mel.Live.Models.Entities;
using Mel.Live.Models.ViewModels;
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
    [ApiController]
    [Route("api/[controller]")]
    public class StoreController : ControllerBase
    {
        #region Properties

        #region Services
        [DeepDependency]
        IMapper Mapper { get; }

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
        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery]DateSearchViewModel model)
        {
            var products = await DataContext.Store.GetAllAsync<Product>(d => true);

            return Ok(Mapper.Map<IEnumerable<ProductViewModel>>(products));
        }
        #endregion

        #endregion
    }
}
