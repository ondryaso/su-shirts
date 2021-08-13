using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SUShirts.Business.Dto;
using SUShirts.Business.Facades;
using SUShirts.Business.Models;

namespace SUShirts.Pages
{
    public class MakeReservation : PageModel
    {
        private readonly ShopFacade _facade;

        public List<ShirtVariantRequestDto> Items { get; set; }

        [BindProperty] public ReservationModel Reservation { get; set; }

        public MakeReservation(ShopFacade facade)
        {
            _facade = facade;
        }

        public async Task<IActionResult> OnGet()
        {
            if (!this.Request.Cookies.TryGetValue("cart", out var cartCookie))
            {
                return this.RedirectToPage("Index");
            }

            var items = await _facade.GetCartItems(cartCookie);
            if (items is null or {Count: 0})
            {
                return this.RedirectToPage("Index");
            }

            this.Items = items;
            return this.Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!this.Request.Cookies.TryGetValue("cart", out var cartCookie))
            {
                return this.RedirectToPage("Index");
            }

            var items = await _facade.GetCartItems(cartCookie);
            if (items is null or {Count: 0})
            {
                return this.RedirectToPage("Index");
            }

            if (!this.ModelState.IsValid)
            {
                this.Items = items;
                return this.Page();
            }

            var result = await _facade.MakeReservation(this.Reservation, cartCookie);
            string resultCode;

            if (result)
            {
                if (items.Any(i => !i.WillBeRequested))
                {
                    resultCode = ReservationResult.ResultVariant.MissingItems.ToString();
                }
                else
                {
                    resultCode = ReservationResult.ResultVariant.Ok.ToString();
                }

                this.Response.Cookies.Append("cart", "", new CookieOptions() {Expires = DateTimeOffset.MinValue});
            }
            else
            {
                resultCode = ReservationResult.ResultVariant.Error.ToString();
            }

            return this.RedirectToPage("ReservationResult", new {state = resultCode});
        }
    }
}
