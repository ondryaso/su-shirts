using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SUShirts.Business.Dto;
using SUShirts.Data;
using SUShirts.Data.Entities;
using SUShirts.Data.Enums;

namespace SUShirts.Business.Facades
{
    public class ShopFacade
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public ShopFacade(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<(List<ShirtDto> Men, List<ShirtDto> Women)> GetOffers()
        {
            var shirts = await _dbContext.Shirts
                .Include(s => s.Variants)
                .OrderBy(s => s.Name)
                .ToListAsync();

            var men = this.GetOffersFor(SexVariant.Man, shirts);
            var women = this.GetOffersFor(SexVariant.Woman, shirts);

            return (men, women);
        }

        private List<ShirtDto> GetOffersFor(SexVariant sex, List<Shirt> shirtEntities)
        {
            var mapped = _mapper.Map<List<ShirtDto>>(shirtEntities);
            var ret = new List<ShirtDto>();

            foreach (var shirtDto in mapped)
            {
                shirtDto.Variants.RemoveAll(variant => variant.Sex != sex && variant.Sex != SexVariant.Unisex);
                if (shirtDto.Variants.Count == 0)
                {
                    continue;
                }

                shirtDto.Variants.Sort((a, b) => (int) a.Size - (int) b.Size);
                ret.Add(shirtDto);
            }

            return ret;
        }
    }
}
