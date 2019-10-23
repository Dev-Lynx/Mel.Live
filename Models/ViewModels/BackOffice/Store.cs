using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mel.Live.Models.ViewModels.BackOffice
{
    public class ProductViewModel
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        public decimal Price { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> Tags { get; set; }

        public ReferencedPhotoViewModel DisplayPicture { get; set; }
    }

    public class ProductPackageViewModel
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> Tags { get; set; }

        public ReferencedPhotoViewModel DisplayPicture { get; set; }
        public decimal DiscountPercentage { get; set; }
        public List<Guid> Products { get; set; }
    }
}
