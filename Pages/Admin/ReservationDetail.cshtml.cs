using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SUShirts.Business.Dto;
using SUShirts.Business.Facades;

namespace SUShirts.Pages.Admin
{
    public class ReservationDetail : PageModel
    {
        private readonly ReservationsFacade _facade;
        public ReservationDto Reservation { get; set; }

        [BindProperty(SupportsGet = true, Name = "id")]
        public int Id { get; set; }

        [TempData] public string ErrorMessage { get; set; }

        public ReservationDetail(ReservationsFacade facade)
        {
            _facade = facade;
        }

        public async Task<IActionResult> OnGet()
        {
            var dto = await _facade.GetDetails(this.Id);
            if (dto is null)
            {
                return this.NotFound();
            }

            this.Reservation = dto;
            return this.Page();
        }

        public async Task<IActionResult> OnGetAddItem(int variantId)
        {
            var result = await _facade.AddItem(this.Id, variantId);
            if (!result)
            {
                this.ErrorMessage = "Při ukládání změny se vyskytla chyba. Zkus to znovu.";
            }

            return this.RedirectToPage();
        }

        public async Task<IActionResult> OnGetRemoveItem(int variantId)
        {
            var result = await _facade.RemoveItem(this.Id, variantId);
            if (!result)
            {
                this.ErrorMessage = "Při ukládání změny se vyskytla chyba. Zkus to znovu.";
            }

            return this.RedirectToPage();
        }

        public async Task<IActionResult> OnGetFinish()
        {
            var result = await _facade.MarkHandled(this.Id, "Test");
            if (!result)
            {
                this.ErrorMessage = "Při ukládání se vyskytla chyba. Zkus to znovu.";
            }

            return this.RedirectToPage();
        }
    }
}
