using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SUShirts.Pages.Admin
{
    public class ReservationDetail : PageModel
    {
        public int Id { get; set; }
        
        public void OnGet(int id)
        {
            this.Id = id;
        }
    }
}
