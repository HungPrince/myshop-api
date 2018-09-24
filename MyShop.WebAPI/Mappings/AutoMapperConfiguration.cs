using AutoMapper;
using MyShop.Model.Models;
using MyShop.WebAPI.Models;
using MyShop.WebAPI.Models.System;

namespace MyShop.WebAPI.Mappings
{
    public class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Permission, PermissionViewModel>();
                cfg.CreateMap<AppUser, AppUserViewModel>();
                cfg.CreateMap<Function, FunctionViewModel>();
                cfg.CreateMap<ProductCategory, ProductCategoryViewModel>();
            });
        }
    }
}