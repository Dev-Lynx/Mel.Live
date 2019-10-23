using System;
using System.Collections.Generic;


namespace Mel.Live.Models.ViewModels
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

        public PhotoViewModel DisplayPicture { get; set; }
    }

    public class ProductPackageViewModel
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public List<string> Tags { get; set; }

        public PhotoViewModel DisplayPicture { get; set; }
        public decimal DiscountPercentage { get; set; }
        public List<Guid> Products { get; set; }
    }

    public class LocationViewModel
    {
        public string City { get; set; }
        public string State { get; set; }
        public string Address { get; set; }

        public string IsDefault { get; set; }
    }


    public class BasicStoreAccountViewModel
    {
        #region Properties
        public int CartProductQuantity { get; set; }
        public List<LocationViewModel> AddressBook { get; set; } = new List<LocationViewModel>();
        #endregion
    }

}