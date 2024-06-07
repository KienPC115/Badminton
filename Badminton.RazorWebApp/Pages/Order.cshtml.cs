using Badminton.Business;
using Badminton.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Badminton.RazorWebApp.Pages
{
    public class OrderModel : PageModel
    {
        private readonly IOrderBusiness _orderBusiness = new OrderBusiness();
        private readonly ICustomerBusiness _customerBusiness = new CustomerBusiness();

        [TempData]
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
        public void OnPostPut(Order order) { this.UpdateOrder(order); }

        private void UpdateOrder(Order order)
        {
            var result = _orderBusiness.UpdateOrder(order);
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

        private void SaveOrder()
        {
            this.Order.OrderId = 0;
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

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var check = _orderBusiness.GetOrderById(id);
            if (check == null)
            {
                this.Message = check.Result.Message;
                return RedirectToPage("/Order");
            }
            var result = await _orderBusiness.DeleteOrder(id);
            if (result != null)
            {
                this.Message = result.Message;
                OnGet();
            }
            else
            {
                this.Message = "Error System";
            }
            return RedirectToPage("/Order");
        }
    }
}
