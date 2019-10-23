using AutoMapper;
using Mel.Live.Data;
using Mel.Live.Extensions.Attributes;
using Mel.Live.Extensions.UnityExtensions;
using Mel.Live.Models.Entities;
using Mel.Live.Models.Entities.BackOffice;
using Mel.Live.Models.ViewModels;
using Mel.Live.Models.ViewModels.BackOffice;
using Mel.Live.Services;
using Mel.Live.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mel.Live.Controllers.BackOffice
{
    [AutoBuild]
    [ApiController]
    [Route("api/backOffice/[controller]")]
    [AuthorizeRoles(UserRoles.Administrator)]
    public class PlatformController : CoreController
    {
        #region Properties

        #region Services
        [DeepDependency]
        MongoDataContext DataContext { get; }

        [DeepDependency]
        IMapper Mapper { get; }

        [DeepDependency]
        ILogger Logger { get; set; }

        [DeepDependency]
        IResourceManager ResourceManager { get; set; }
        #endregion

        #endregion

        #region Methods
        [HttpPost("add")]
        public async Task<IActionResult> CreatePlatform([FromBody]NewPlatformViewModel model)
        {
            Platform platform = Mapper.Map<Platform>(model);
            await platform.InitializeAsync();

            await DataContext.Store.AddOneAsync(platform);

            return Ok();
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdatePlatform([FromBody]PlatformEditViewModel model)
        {
            Platform platform = await DataContext.Store.GetOneAsync<Platform>(p => p.Id == model.Id);

            if (platform == null) return NotFound("No platform exists with the given Id");

            platform.Update(Mapper.Map<Platform>(model));

            if (!await DataContext.Store.UpdateOneAsync(platform))
                return BadRequest("An error occured while updating the platform");

            return Ok();
        }

        [HttpPost("upload/photo")]
        public async Task<IActionResult> UploadPlatformPhoto([FromBody]ResourceUploadViewModel model)
        {
            Platform platform = await DataContext.Store.GetOneAsync<Platform>(p => p.Id == model.ReferenceId);

            if (platform == null) return NotFound("No platform exists with the given Id");

            var result = await ResourceManager.UploadPhoto(new FileUploadContext()
            {
                FileName = model.File.FileName,
                Folder = @"Company\Platforms",
                Stream = model.File.OpenReadStream()
            });

            if (!result.Success) return BadRequest(result.Message);

            Photo photo = result.Entity;
            photo._Reference = platform.Id;
            platform.Resources.Add(photo);

            if (platform._DisplayPicture == Guid.Empty) platform._DisplayPicture = photo.Id;

            await DataContext.Store.UpdateOneAsync(platform);

            return Created(photo.Url, null);
        }

        [HttpPost("update/logo")]
        public async Task<IActionResult> UpdateLogo(DisplayPictureEditViewModel model)
        {
            Platform platform = await DataContext.Store.GetOneAsync<Platform>(p => p.Id == model.Id);

            var photo = platform.Resources.FirstOrDefault(r => r.Id == model.ResourceId);

            if (photo == null)
                return NotFound("Failed to locate photo");

            platform._DisplayPicture = photo.Id;

            if (!await DataContext.Store.UpdateOneAsync(platform))
                return BadRequest("An error occured while updating the platform.");

            return Ok();
        }
        #endregion
    }
}
