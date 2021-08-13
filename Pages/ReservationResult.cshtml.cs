using System;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SUShirts.Pages
{
    public class ReservationResult : PageModel
    {
        public enum ResultVariant
        {
            Ok,
            Error,
            MissingItems,
            BadRequest
        }

        public ResultVariant Result { get; set; }

        public void OnGet(string state)
        {
            try
            {
                this.Result = Enum.Parse<ResultVariant>(state, true);
            }
            catch
            {
                this.Result = ResultVariant.BadRequest;
            }
        }
    }
}
