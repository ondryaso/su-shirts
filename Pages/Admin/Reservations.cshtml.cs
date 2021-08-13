using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SUShirts.Business.Dto;
using SUShirts.Business.Facades;

namespace SUShirts.Pages.Admin
{
    public class Reservations : PageModel
    {
        private readonly ReservationsFacade _facade;

        public List<ReservationDto> PendingReservations { get; set; }
        public List<ReservationDto> FinishedReservations { get; set; }

        public Reservations(ReservationsFacade facade)
        {
            _facade = facade;
        }

        public async Task OnGet()
        {
            var dtos = await _facade.GetAll();
            this.PendingReservations = dtos.FindAll(r => !r.Handled);
            this.FinishedReservations = dtos.FindAll(r => r.Handled);
        }
    }
}
