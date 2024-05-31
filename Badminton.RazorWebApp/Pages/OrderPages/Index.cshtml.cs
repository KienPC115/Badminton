using Badminton.Business;
using Badminton.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Badminton.RazorWebApp.Pages.OrderPages
{
    public class IndexModel : PageModel
    {
        private readonly IOrderBusiness _OrderBusiness = new OrderBusiness();
        public string Message { get; set; } = default;
        [BindProperty]
        public Order Order { get; set; } = default;
        public List<Order> Orders { get; set; } = new List<Order>();

        public void OnGet()
        {
            Orders = this.GetOrders().ToList();
        }

        public void OnPost()
        {
            this.SaveOrder();
        }

        public void OnDelete()
        {
        }


        private List<Order> GetOrders()
        {
            try
            {
                var OrderResult = _OrderBusiness.GetAllOrders();

                if (OrderResult.Status > 0 && OrderResult.Result.Data != null)
                {
                    var Orders = (List<Order>)OrderResult.Result.Data;
                    return Orders;
                }
                return new List<Order>();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void SaveOrder()
        {
            var OrderResult = _OrderBusiness.Save(this.Order);

            if (OrderResult != null)
            {
                this.Message = OrderResult.Result.Message;
            }
            else
            {
                this.Message = "Error system";
            }
        }
    }
}
