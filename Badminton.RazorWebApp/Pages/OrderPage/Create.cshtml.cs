using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Badminton.Data.Models;
using Badminton.Business;
using Badminton.Common;
using Badminton.Business.Shared;

namespace Badminton.RazorWebApp.Pages.OrderPage
{
    public class CreateModel : PageModel
    {
        private readonly IOrderBusiness _orderBusiness;
        private readonly ICustomerBusiness _customerBusiness;
        public CreateModel()
        {
            _customerBusiness ??= new CustomerBusiness();
            _orderBusiness ??= new OrderBusiness();
        }

        public IActionResult OnGet()
        {
            if (Helpers.GetValueFromSession("cus", out Customer cus, HttpContext) && cus.Name.Equals("admin") && cus.Email.Equals("admin@example.com"))
            {
                var result = _customerBusiness.GetAllCustomers();
                Customer = result.Result.Data as List<Customer>;
                Status = OrderShared.Status();
                Type = OrderShared.Type();

                Order = new Order();
                Order.ModifiedDate = DateTime.Now;
                Order.OrderDate = DateTime.Now;
                return Page();
            }
            return RedirectToPage("../Index");
        }

        [BindProperty]
        public Order Order { get; set; } = default!;
        [BindProperty]
        public List<string> Type { get; set; }
        public List<Customer> Customer { get; set; }
        [BindProperty]
        public List<string> Status { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (Order == null)
                {
                    OnGet();
                    return Page();
                }
                this.Order.OrderNotes ??= string.Empty;
                var result = await _orderBusiness.AddOrders(this.Order);
                if (result.Status <= 0)
                {
                    OnGet();
                    TempData["message"] = result.Message;
                    return Page();
                }

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                TempData["message"] = ex.Message;
                OnGet();
                return Page();
            }
        }
    }
}
