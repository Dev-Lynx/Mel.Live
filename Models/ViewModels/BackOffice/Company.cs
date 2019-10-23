using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Mel.Live.Models.ViewModels.BackOffice
{
    public class CompanyViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public List<string> PhoneNumbers { get; set; }
    }

    public class CompanyLogoUpdateViewModel
    {
        [Required]
        public Guid ResourceId { get; set; }
    }


    #region Platform
    public class NewPlatformViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class PlatformEditViewModel : NewPlatformViewModel
    {
        public Guid Id { get; set; }
    }
    #endregion
}
