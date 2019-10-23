using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Mel.Live.Data;
using Mel.Live.Extensions.UnityExtensions;
using Mel.Live.Models.Entities;
using Mel.Live.Models.Options;
using Mel.Live.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDbGenericRepository.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Mel.Live.Services
{
    [AutoBuild]
    public class ResourceManager : IResourceManager
    {
        #region Properties

        #region Services
        [DeepDependency]
        ILogger Logger { get; }

        [DeepDependency]
        MongoDataContext DataContext { get; }

        [DeepDependency]
        Cloudinary Cloudinary { get; }                                                                                                             
        #endregion

        #region Options
        [DeepDependency(TargetType = typeof(IOptions<CloudinaryOptions>), TargetProperty = nameof(IOptions<CloudinaryOptions>.Value))]
        CloudinaryOptions CloudinaryOptions { get; set; }
        #endregion

        #endregion

        #region Methods
        public async Task<FileUploadResult<Photo>> UploadPhoto(FileUploadContext context)
        {
            RawUploadResult result = new ImageUploadResult();

            using (context.Stream)
            {
                var @params = new ImageUploadParams()
                {
                    File = new FileDescription(context.FileName, context.Stream),
                    Folder = Core.CLOUDINARY_BASE_FOLDER + "/" + context.Folder
                };

                result = await Cloudinary.UploadAsync(@params);
            }

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
                return new FileUploadResult<Photo>
                {
                    Success = false,
                    Message = $"Failed to upload image.\n{result.Error.Message}"
                };
            

            return new FileUploadResult<Photo>
            {
                Entity = new Photo()
                {
                    Name = context.FileName,
                    Url = result.Uri.OriginalString,
                    Signature = result.Signature
                },
                Success = true,
                Url = result.Uri.OriginalString,
            };
        }
        #endregion
    }

    public class FileUploadContext
    {
        public string Folder { get; set; }
        public string FileName { get; set; }
        public Stream Stream { get; set; }
    }

    public class FileUploadResult<T> where T : Document
    {
        public T Entity { get; set; }
        public bool Success { get; set; }
        public string Url { get; set; }
        public string Message { get; set; }
    }
}
