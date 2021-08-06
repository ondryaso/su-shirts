using AutoMapper;
using SUShirts.Business.Dto;
using SUShirts.Data.Entities;

namespace SUShirts.Business.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            this.CreateMap<Color, ColorDto>();
            this.CreateMap<ShirtVariant, ShirtVariantDto>();
            this.CreateMap<Shirt, ShirtDto>()
                .ForMember(s => s.ImageUrl, options =>
                    options.MapFrom(m => m.ImageUrl ?? ("/images/shirts/" + m.Id + ".jpg")));
        }
    }
}
