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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mel.Live.Controllers.BackOffice
{
    [Route("api/backOffice/[controller]")]
    [ApiController]
    [AutoBuild]
    [AuthorizeRoles(UserRoles.Administrator)]
    public class CompanyController : CoreController
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
        [HttpPost("update")]
        public async Task<IActionResult> UpdateCompany([FromBody]CompanyViewModel model)
        {
            var company = Mapper.Map<Company>(model);
            DataContext.Company.Update(company);

            if (!await DataContext.Store.UpdateOneAsync(DataContext.Company))
                return BadRequest("An error occured while updaing the company");

            return Ok();
        }

        [HttpPost("upload/photo")]
        public async Task<IActionResult> UploadCompanyPhoto([FromBody]ResourceUploadViewModel model)
        {
            Company company = DataContext.Company;
            var result = await ResourceManager.UploadPhoto(new FileUploadContext()
            {
                FileName = model.File.FileName,
                Folder = "Company",
                Stream = model.File.OpenReadStream()
            });

            if (!result.Success) return BadRequest(result.Message);

            Photo photo = result.Entity;
            photo._Reference = company.Id;
            company.Resources.Add(photo);

            if (company._Logo == Guid.Empty) company._Logo = photo.Id;

            await DataContext.Store.UpdateOneAsync(company);

            return Created(photo.Url, null);
        }

        [HttpPost("update/logo")]
        public async Task<IActionResult> UpdateLogo(CompanyLogoUpdateViewModel model)
        {
            Company company = DataContext.Company;

            var photo = company.Resources.FirstOrDefault(r => r.Id == model.ResourceId);

            if (photo == null)
                return NotFound("Failed to locate photo");

            company._Logo = photo.Id;

            if (!await DataContext.Store.UpdateOneAsync(company))
                return BadRequest("An error occured while updating the company.");

            return Ok();
        }
        #endregion
    }
}
