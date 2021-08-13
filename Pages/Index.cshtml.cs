using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SUShirts.Business.Facades;
using SUShirts.Data.Enums;
using SUShirts.Models;

namespace SUShirts.Pages
{
    public class Index : PageModel
    {
        private readonly ShopFacade _facade;

        public ShirtListModel ManShirts { get; set; }
        public ShirtListModel WomanShirts { get; set; }

        public Index(ShopFacade facade)
        {
            _facade = facade;
        }

        public async Task<IActionResult> OnGet()
        {
            this.ViewData["ShowCart"] = true;
            
            var (men, women) = await _facade.GetOffers();

            this.ManShirts = new ShirtListModel() {Shirts = men, Sex = SexVariant.Man};
            this.WomanShirts = new ShirtListModel() {Shirts = women, Sex = SexVariant.Woman};

            return this.Page();
        }
    }
}
