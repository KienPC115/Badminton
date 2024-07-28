using Badminton.Business;
using Badminton.Business.Interface;
using Badminton.Business.Shared;
using Badminton.Common;
using Badminton.Data;
using Badminton.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Badminton.RazorWebApp.Pages
{
    public class PaymentPageModel : PageModel
    {
        private IOrderBusiness _orderBusiness;
        public PaymentPageModel()
        {
            _orderBusiness = new OrderBusiness();
        }
        public async Task<IActionResult> OnGet()
        {
            try
            {
                var vnpay = new VnPayLibrary();

                // Access query parameters from HttpContext.Request.Query
                var queryCollection = HttpContext.Request.Query;
                foreach (var (key, value) in queryCollection)
                {
                    if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(key, value.ToString());
                    }
                }

                // Extract necessary parameters
                var vnp_OrderId = vnpay.GetResponseData("vnp_TxnRef");
                var vnp_TransactionId = vnpay.GetResponseData("vnp_TransactionNo");
                var vnp_SecureHash = queryCollection.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
                var vnp_responseCode = vnpay.GetResponseData("vnp_ResponseCode");
                var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");
                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, "BYKJBHPPZKQMKBIBGGXIYKWYFAYSJXCW");

                // Extract order ID from OrderInfo
                var orderId = vnp_OrderInfo.Substring(vnp_OrderInfo.IndexOf(":") + 1);

                if (!vnp_responseCode.Equals("00"))
                {
                    var result = await _orderBusiness.GetOrderById(int.Parse(orderId));
                    var listType = OrderShared.Type();
                    var status = OrderShared.Status();
                    if (result.Status <= 0)
                    {
                        TempData["message"] = result.Message;
                        return RedirectToPage("./OrderPage/Index");
                    }

                    var order = result.Data as Order;
                    order.Type = listType[0];
                    order.OrderStatus = status[2];
                    result = await _orderBusiness.UpdateOrder(order);
                    if (result.Status <= 0)
                    {
                        TempData["message"] = result.Message;
                        return RedirectToPage("./OrderPage/Index");
                    }
                }

                return RedirectToPage("./OrderDetailPage/Index", new { orderID = orderId });
            }
            catch (Exception ex)
            {
                TempData["message"] = ex.Message;
                return RedirectToPage("./OrderPage/Index");
            }
        }

        public void OnPost()
        {

        }
    }
}
