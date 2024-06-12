using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Badminton.Business;
using Badminton.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Badminton.RazorWebApp.Pages.CustomerPage {
    public class EditModel : PageModel {
        private readonly ICustomerBusiness _business;

        public EditModel() {
            _business ??= new CustomerBusiness();
        }

        [BindProperty]
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

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }


            try {
                await _business.UpdateCustomer(Customer.CustomerId, Customer);
            } catch (DbUpdateConcurrencyException) {
                if (!(await CustomerExists(Customer.CustomerId))) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private async Task<bool> CustomerExists(int id) {
            var result = await _business.GetCustomerById((int)id);
            return result.Data != null;
        }
    }
}
