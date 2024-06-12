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
    public class DetailsModel : PageModel {
        private readonly ICustomerBusiness _business;

        public DetailsModel() {
            _business ??= new CustomerBusiness();
        }

        public Customer Customer { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id) {
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
