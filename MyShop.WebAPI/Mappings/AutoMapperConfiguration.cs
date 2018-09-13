using AutoMapper;
using MyShop.Model.Models;
using MyShop.WebAPI.Models;

namespace MyShop.WebAPI.Mappings
{
    public class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Permission, PermissionViewModel>();
            });
        }
    }
}