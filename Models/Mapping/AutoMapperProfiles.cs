using AutoMapper;
using Mel.Live.Models.Entities;
using Mel.Live.Models.Responses;
using Mel.Live.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mel.Live.Models.Mapping
{
    public class ViewModelToEntityProfile : Profile
    {
        public ViewModelToEntityProfile()
        {
            CreateMap<RegisterViewModel, User>()
                .ForMember(o => o.Referrer, opt => opt.AllowNull());


            CreateMap<ReferrerViewModel, Referrer>();
            CreateMap<ProductViewModel, Product>().ForMember(o => o.Id, opt => opt.Ignore());
            CreateMap<ProductPackageViewModel, Product>().ForMember(o => o.Id, opt => opt.Ignore());
        }
    }


    public class EntityToViewModelProfile : Profile
    {
        public EntityToViewModelProfile()
        {
            CreateMap<User, UserViewModel>()
                .ForMember(o => o.Rank, opt => opt.AllowNull())
                .ForMember(o => o.Referrer, opt => opt.AllowNull())
                .ForMember(o => o.DisplayPicture, opt => opt.AllowNull());

            CreateMap<Node, NodeViewModel>();
            CreateMap<UserRank, UserRankViewModel>();
            CreateMap<Referrer, ReferrerViewModel>();
            CreateMap<StoreAccount, BasicStoreAccountViewModel>();
            CreateMap<Photo, ReferencedPhotoViewModel>();
            CreateMap<Photo, PhotoViewModel>();
            CreateMap<Product, ProductViewModel>().ForMember(o => o.DisplayPicture, opt => opt.AllowNull());


            CreateMap<DepositViewModel, BankTransactionRequest>().ForMember(o => o.Meta, 
                opt => opt.MapFrom((s, d) => 
            {
                return new BankTransactionMetadata()
                {
                    BankAccountId = s.BankAccountId,
                    CompanyAccountId = s.CompanyBankAccountId,
                    Description = s.Description,
                };
            }));

            CreateMap<WithdrawalViewModel, BankTransactionRequest>().ForMember(o => o.Meta,
                opt => opt.MapFrom((s, d) =>
                {
                    return new BankTransactionMetadata()
                    {
                        BankAccountId = s.BankAccountId,
                    };
                }));
        }
    }
}
