using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Badminton.Data.Models;
using Badminton.Business;
using Badminton.Common;
using Badminton.Business.Shared;

namespace Badminton.RazorWebApp.Pages.OrderPage
{
    public class EditModel : PageModel
    {
        private readonly IOrderBusiness _orderBusiness;
        private readonly ICustomerBusiness _customerBusiness;
        public EditModel()
        {
            _customerBusiness ??= new CustomerBusiness();
            _orderBusiness ??= new OrderBusiness();
        }

        [BindProperty]
        public Order Order { get; set; } = default!;
        [BindProperty]
        public List<string> Type { get; set; }
        [BindProperty]
        public List<string> Status { get; set; }
        public List<Customer> Customer { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if(Helpers.GetValueFromSession("cus", out Customer cus, HttpContext) && cus.Name.Equals("admin") && cus.Email.Equals("admin@example.com"))
            {
                var result = await _customerBusiness.GetAllCustomers();
                Customer = result.Data as List<Customer>;

                result = await _orderBusiness.GetOrderById(id.Value);
                if (result.Status < 0)
                {
                    return RedirectToPage("../Index");
                }
                Status = OrderShared.Status();
                Type = OrderShared.Type();
                Order = (result.Data as Order);
                Order.ModifiedDate = DateTime.Now;
                if (Order == null)
                {
                    return RedirectToPage("../Index");
                }
                return Page();
            }
            return RedirectToPage("../Index");
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {

            var result = await _orderBusiness.UpdateOrder(Order);
            if (result.Status <= 0)
            {
                OnGetAsync(Order.OrderId);
                return Page();
            }

            return RedirectToPage("./Index");
        }

    }
}
