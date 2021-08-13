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
                    options.MapFrom(m => $"/images/shirts/{(m.ImageUrl ?? (m.Id + ".jpg"))}"));
            this.CreateMap<Shirt, ShirtVariantRequestDto>();
            this.CreateMap<ShirtVariant, ShirtVariantRequestDto>()
                .ForMember(s => s.RequestedCount, options =>
                    options.Ignore())
                .ForMember(s => s.WillBeRequested, options =>
                    options.Ignore())
                .IncludeMembers(s => s.Shirt);
        }
    }
}
