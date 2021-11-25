using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SUShirts.Business.Dto;
using SUShirts.Business.Facades;

namespace SUShirts.Pages.Admin
{
    public class ReservationDetail : PageModel
    {
        public class InternalInfoModel
        {
            public string Note { get; set; }
            public string AssignedTo { get; set; }

            public InternalInfoModel()
            {
            }

            public InternalInfoModel(ReservationDto reservationDto)
            {
                this.Note = reservationDto.InternalNote;
                this.AssignedTo = reservationDto.AssignedTo;
            }
        }

        private readonly ReservationsFacade _facade;
        public ReservationDto Reservation { get; set; }

        [BindProperty(SupportsGet = true, Name = "id")]
        public int Id { get; set; }

        [TempData] public string ErrorMessage { get; set; }

        [BindProperty] public InternalInfoModel InternalInfo { get; set; }

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
            this.InternalInfo = new InternalInfoModel(dto);

            return this.Page();
        }

        public async Task<IActionResult> OnPostSetInternalInfo()
        {
            var result = await _facade.SetInternalInfo(this.Id, this.InternalInfo.AssignedTo, this.InternalInfo.Note);
            if (!result)
            {
                this.ErrorMessage = "Při ukládání změny se vyskytla chyba. Zkus to znovu.";
            }

            return this.RedirectToPage();
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
            var result = await _facade.MarkHandled(this.Id, this.UserIdentifier);
            if (!result)
            {
                this.ErrorMessage = "Při ukládání se vyskytla chyba. Zkus to znovu.";
            }

            return this.RedirectToPage();
        }

        public async Task<IActionResult> OnGetCancel()
        {
            var result = await _facade.Cancel(this.Id, this.UserIdentifier);
            if (!result)
            {
                this.ErrorMessage = "Při ukládání se vyskytla chyba. Zkus to znovu.";
            }

            return this.RedirectToPage();
        }

        private string UserIdentifier => this.User.FindFirstValue(ClaimTypes.Name)
                                         ?? this.User.FindFirstValue(ClaimTypes.Email);
    }
}
