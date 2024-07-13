using Badminton.Business;
using Badminton.Business.Shared;
using Badminton.Common;
using Badminton.Data.Models;
using Humanizer.Localisation.TimeToClockNotation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;

namespace Badminton.RazorWebApp.Pages
{
    public class CartPageModel : PageModel
    {
        public IList<CourtDetail> Cart { get; set; }
        private readonly ICourtDetailBusiness _courtDetailBusiness;
        private readonly IOrderBusiness _orderBusiness;
        private readonly IHubContext<SignalrServer> _hubContext;

        public CartPageModel(IHubContext<SignalrServer> hubContext)
        {
            _hubContext = hubContext;
            _courtDetailBusiness ??= new CourtDetailBusiness();
            _orderBusiness ??= new OrderBusiness();
        }

        public void OnGet(string? checkout, int? courtDetailID)
        {
            if (Helpers.GetValueFromSession("cart", out List<int> cart, HttpContext))
            {
                GetCart(cart);
            }

            if (!checkout.IsNullOrEmpty())
            {
                Checkout(cart);
            }
            if (courtDetailID != null && courtDetailID != 0)
            {
                RemoveOutCart(courtDetailID.Value);
            }
        }

        private void Checkout(List<int> cart)
        {
            if (Helpers.GetValueFromSession("cus", out Customer cus, HttpContext))
            {
                var status = CourtDetailShared.Status();
                var availableCourt = Cart.ToList().Where(c => c.Status.Equals(status[0])).ToList();

                var result = _orderBusiness.Checkout(availableCourt, cus.CustomerId);
                if (result.Status <= 0)
                {
                    TempData["message"] = "Something failed while checkout.";
                    return;
                }
                TempData["message"] = "Checkout successfully";
                _hubContext.Clients.All.SendAsync("ChangeStatusCourtDetail", cus.Name, availableCourt);
                Cart.Clear();
                cart.Clear();
                Helpers.SetValueToSession("cart", cart, HttpContext);
            }
        }

        private void GetCart(List<int> cart)
        {
            var result = _courtDetailBusiness.GetCourtDetailsIncludeCourt(cart);
            if (result.Status <= 0)
            {
                TempData["message"] = result.Message;
                return;
            }
            var courtDetails = result.Data as List<CourtDetail>;
            Cart = courtDetails;
        }

        private bool RemoveOutCart(int courtDetailID)
        {
            try
            {
                if (!Helpers.GetValueFromSession("cart", out List<int> curCart, HttpContext))
                {
                    TempData["message"] = "Remove failed";
                    return false;
                }
                if (!curCart.Remove(courtDetailID))
                {
                    return true;
                }
                GetCart(curCart);
                Helpers.SetValueToSession("cart", curCart, HttpContext);
                return true;
            }
            catch (Exception ex)
            {
                TempData["message"] = ex.Message;
                return false;
            }
        }

        public IActionResult OnPostAsync(int courtDetailID)
        {
            //if it's new, init cart.
            if (!Helpers.GetValueFromSession("cart", out List<int> cart, HttpContext))
            {
                cart = new List<int>();
            }
            
            if (cart.Contains(courtDetailID))
            {
                TempData["message"] = "This court detail is already existed in cart";
                return RedirectToAction("./CourtDetailPage/Index");
            }
            cart.Add(courtDetailID);
            Helpers.SetValueToSession("cart", cart, HttpContext);
            return RedirectToPage("./CourtDetailPage/Index");
        }
    }
}
