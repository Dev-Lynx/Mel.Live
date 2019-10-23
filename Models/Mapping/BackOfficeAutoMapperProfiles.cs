using AutoMapper;
using Mel.Live.Models.Entities;
using Mel.Live.Models.ViewModels.BackOffice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mel.Live.Models.Mapping
{
    public class BackOfficeViewModelToEntityProfile : Profile
    {
        public BackOfficeViewModelToEntityProfile()
        {
            CreateMap<ProductViewModel, Product>().ForMember(o => o.Id, opt => opt.Ignore());
            CreateMap<ProductPackageViewModel, ProductPackage>().ForMember(o => o.Id, opt => opt.Ignore());
        }
    }

    public class BackOfficeEntityToViewModelProfile : Profile
    {
        public BackOfficeEntityToViewModelProfile()
        {
            CreateMap<Product, ProductViewModel>().ForMember(o => o.DisplayPicture, opt => opt.AllowNull());
        }
    }
}
