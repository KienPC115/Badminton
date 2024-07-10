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
            if (!Helpers.GetValueFromSession("cart", out List<CourtDetail> cart, HttpContext))
            {
                cart = new List<CourtDetail>();
            }
            Cart = cart;
            if (!checkout.IsNullOrEmpty())
            {
                if (Helpers.GetValueFromSession("cus", out Customer cus, HttpContext))
                {
                    var result = _orderBusiness.Checkout(cart, cus.CustomerId);
                    if (result.Status <= 0)
                    {
                        TempData["message"] = "Something failed while checkout.";
                    }
                    else
                    {
                        _hubContext.Clients.All.SendAsync("ChangeStatusCourtDetail");
                        TempData["message"] = "Checkout successfully";
                        cart.Clear();
                        Cart = cart;
                        Helpers.SetValueToSession("cart", cart, HttpContext);
                    }
                }
            }
            if (courtDetailID != null && courtDetailID != 0)
            {
                RemoveOutCart(courtDetailID.Value);
            }
        }

        private bool RemoveOutCart(int courtDetailID)
        {
            try
            {
                if (!Cart.Remove(Cart.FirstOrDefault(c => c.CourtDetailId == courtDetailID)))
                {
                    TempData["message"] = "Remove failed";
                    return false;
                }
                Helpers.SetValueToSession("cart", Cart, HttpContext);
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
            if (!Helpers.GetValueFromSession("cart", out List<CourtDetail> cart, HttpContext))
            {
                cart = new List<CourtDetail>();
            }

            if (cart.FirstOrDefault(c => c.CourtDetailId == courtDetailID) != null)
            {
                TempData["message"] = "This court detail is already existed in cart";
                return RedirectToAction("./CourtDetailPage/Index");
            }

            var result = _courtDetailBusiness.GetCourtDetailIncludeCourt(courtDetailID);
            if (result.Status != Const.SUCCESS_READ_CODE)
            {
                TempData["message"] = result.Message;
                return RedirectToAction("./CourtDetailPage/Index");
            }
            var courtDetail = result.Data as CourtDetail;
            
            List<string> status = CourtDetailShared.Status();
            if (!courtDetail.Status.Equals(status[0]))
            {
                TempData["message"] = $"The status of this court detail is not {status[0]}";
                return RedirectToPage("./CourtDetailPage/Index");
            }
            cart.Add(courtDetail);
            Helpers.SetValueToSession("cart", cart, HttpContext);
            return RedirectToPage("./CourtDetailPage/Index");
        }
    }
}
