using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SUShirts.Business.Dto;
using SUShirts.Business.Facades;
using SUShirts.Data.Enums;

namespace SUShirts.Pages
{
    public class Index : PageModel
    {
        private readonly ShopFacade _facade;

        public List<ShirtDto> ManShirts { get; set; }
        public List<ShirtDto> WomanShirts { get; set; }

        public Index(ShopFacade facade)
        {
            _facade = facade;
        }

        public async Task<IActionResult> OnGet()
        {
            var (men, women) = await _facade.GetOffers();

            this.ManShirts = men;
            this.WomanShirts = women;

            return this.Page();
        }
    }
}
