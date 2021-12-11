using AutoMapper;
using ecommerce.Data;

namespace ecommerce.FrontEnd.Models.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<VendorApplication, MVVendorApplication>();
            CreateMap<ApplicationUser, MVApplicationUser>().ReverseMap();
        }
    }
}
