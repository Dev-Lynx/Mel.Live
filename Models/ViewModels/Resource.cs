using Mel.Live.Models.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Mel.Live.Models.ViewModels
{
    public class PhotoViewModel
    {
        [Required]
        public string Url { get; set; }
        public PhotoType Type { get; set; }
    }

    public class ReferencedPhotoViewModel : PhotoViewModel
    {
        public Guid Id { get; set; }
    }

    public class ResourceUploadViewModel
    {
        #region Properties
        public Guid ReferenceId { get; set; }
        public IFormFile File { get; set; }
        #endregion
    }
}
