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

            this.CreateMap<Shirt, ReservationItemDto>()
                .ForMember(ri => ri.ShirtId, options =>
                    options.MapFrom(s => s.Id))
                .ForMember(ri => ri.ShirtName, options =>
                    options.MapFrom(s => s.Name));
            this.CreateMap<ShirtVariant, ReservationItemDto>()
                .ForMember(ri => ri.ShirtSex, options =>
                    options.MapFrom(sv => sv.Sex))
                .ForMember(ri => ri.ShirtSize, options =>
                    options.MapFrom(sv => sv.Size))
                .ForMember(ri => ri.ShirtVariantId, options =>
                    options.MapFrom(sv => sv.Id))
                .ForMember(ri => ri.InStock, options =>
                    options.MapFrom(sv => sv.ItemsInStock))
                .ForMember(ri => ri.LeftForReserving, options =>
                    options.MapFrom(sv => sv.ItemsLeft));
            this.CreateMap<ReservationItem, ReservationItemDto>()
                .IncludeMembers(s => s.Reservation)
                .IncludeMembers(s => s.ShirtVariant);
            this.CreateMap<Reservation, ReservationDto>();
        }
    }
}
