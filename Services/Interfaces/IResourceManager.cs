using Mel.Live.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mel.Live.Services.Interfaces
{
    public interface IResourceManager
    {
        Task<FileUploadResult<Photo>> UploadPhoto(FileUploadContext context);
    }
}
