using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Badminton.Business;
using Badminton.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Badminton.RazorWebApp.Pages.CustomerPage {
    public class DetailsModel : CustomPage {
        private readonly ICustomerBusiness _business;

        public DetailsModel() {
            _business ??= new CustomerBusiness();
        }

        public Customer Customer { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id) {
            IsAdmin = CheckAdmin();

            if (!IsAdmin) {
                TempData["message"] = "You don't have enough permission.";
                return RedirectToPage("./Index");
            }

            if (id == null) {
                return NotFound();
            }

            var customer = await _business.GetCustomerById((int)id);
            if (customer == null) {
                return NotFound();
            }
            Customer = customer.Data as Customer;
            return Page();
        }
    }
}
