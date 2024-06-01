using Badminton.Business;
using Badminton.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Badminton.RazorWebApp.Pages
{
    public class OrderModel : PageModel
    {
        private readonly IOrderBusiness _orderBusiness = new OrderBusiness();
        private readonly ICustomerBusiness _customerBusiness = new CustomerBusiness();
        public string Message { get; set; } = default;

        [BindProperty]
        public Order Order { get; set; }

        public List<Customer> CustomersList { get; set; } = new List<Customer>();

        public List<Order> Orders { get; set; } = new List<Order>();

        public void OnGet()
        {
            Orders = this.GetOrders();
            CustomersList = this.GetCustomers();
        }

        private List<Customer> GetCustomers()
        {
            var result = _customerBusiness.GetAllCustomers();
            if (result.Result.Data == null)
            {
                return new();
            }
            var customers = (List<Customer>)result.Result.Data;
            return customers;
        }

        private List<Order> GetOrders()
        {
            var orderResult = _orderBusiness.GetAllOrders();
            if (orderResult.Result == null)
            {
                return new List<Order>();
            }
            var orders = (List<Order>)orderResult.Result.Data;
            return orders;
        }

        public void OnPost() { this.SaveOrder(); }

        private void SaveOrder()
        {

            var result = _orderBusiness.AddOrders(this.Order);
            if (result != null)
            {
                this.Message = result.Result.Message;
                OnGet();
            }
            else
            {
                this.Message = "Error System";
            }
        }
    }
}
