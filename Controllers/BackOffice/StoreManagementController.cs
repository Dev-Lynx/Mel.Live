using AutoMapper;
using Mel.Live.Data;
using Mel.Live.Extensions.UnityExtensions;
using Mel.Live.Models.Entities;
using Mel.Live.Models.ViewModels.BackOffice;
using Mel.Live.Services;
using Mel.Live.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Mel.Live.Controllers.BackOffice
{
    [AutoBuild]
    [ApiController]
    [Route("api/[controller]")]
    public class StoreManagementController : ControllerBase
    {
        #region Properties
        [DeepDependency]
        IMapper Mapper { get; }

        [DeepDependency]
        ILogger Logger { get; }

        [DeepDependency]
        MongoDataContext DataContext { get; }

        [DeepDependency]
        IResourceManager ResourceManager { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Add a product
        /// </summary>
        /// <param name="model"></param>
        [HttpPost("add/product")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(OkResult), Description = "Product was successfully created")]
        public async Task<IActionResult> AddProduct(ProductViewModel model)
        {
            var product = Mapper.Map<Product>(model);
            await product.InitializeAsync();

            if (DataContext.Store.Any<Product>(p => p.Name == model.Name))
                return BadRequest("A product already exists with this name.");

            await DataContext.Store.AddOneAsync(product);

            return Created(product.Name, Mapper.Map<ProductViewModel>(product));
        }

        /// <summary>
        /// Create a product package
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("add/package")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(OkResult), Description = "Product package was successfully created")]
        public async Task<IActionResult> CreateProductPackage(ProductPackageViewModel model)
        {
            var package = Mapper.Map<ProductPackage>(model);
            await package.InitializeAsync();

            await DataContext.Store.AddOneAsync(package);

            return Ok();
        }

        [HttpPost("update/product")]
        public async Task<IActionResult> UpdateProduct(ProductViewModel model)
        {
            Product product = await DataContext.Store.GetByIdAsync<Product>(model.Id);
            product.Update(Mapper.Map<Product>(model));

            if (!await DataContext.Store.UpdateOneAsync(product))
                return BadRequest("An error occured while updating a product.");

            return Ok();
        }

        [HttpPost("upate/dp/product")]
        public async Task<IActionResult> UpdateDisplayPicture(DisplayPictureEditViewModel model)
        {
            Product product = await DataContext.Store.GetByIdAsync<Product>(model.Id);

            if (product == null)
                return NotFound("Product could not be located.");

            var photo = product.Resources.FirstOrDefault(r => r.Id == model.ResourceId) as Photo;

            if (photo == null)
                return NotFound("Failed to locate photo.");

            product._DisplayPicture = photo.Id;

            if (!await DataContext.Store.UpdateOneAsync(product))
                return BadRequest("An error occured while updating a product.");

            return Ok();
        }

        /// <summary>
        /// Upload resources for a product
        /// </summary>
        /// <param name="model">Resource Upload Wrapper</param>
        [HttpPost("upload/product")]
        public async Task<IActionResult> UploadProductPhoto([FromForm]Models.ViewModels.ResourceUploadViewModel model)
        {
            Product product = await DataContext.Store.GetOneAsync<Product>(p => p.Id == model.ReferenceId);

            if (product == null)
                return NotFound("Product could not be located.");

            var result = await ResourceManager.UploadPhoto(new FileUploadContext()
            {
                FileName = model.File.FileName,
                Folder = "Products",
                Stream = model.File.OpenReadStream()
            });

            if (!result.Success)
                return BadRequest(result.Message);

            Photo photo = result.Entity;
            photo._Reference = product.Id;
            product.Resources.Add(photo);

            if (product._DisplayPicture == Guid.Empty)
                product._DisplayPicture = photo.Id;

            await DataContext.Store.UpdateOneAsync(product);

            return Created(photo.Url, null);
        }

        [HttpGet("store")]
        public async Task<IActionResult> GetProducts(Models.ViewModels.DateRangeSieveModel model)
        {
            var products = await DataContext.Store.GetAllAsync<Product>(p => true);
            return Ok(Mapper.Map<IEnumerable<ProductViewModel>>(products));
        }
        #endregion
    }
}
