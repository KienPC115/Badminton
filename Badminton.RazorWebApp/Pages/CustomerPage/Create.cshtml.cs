using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Badminton.Business;
using Badminton.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Badminton.RazorWebApp.Pages.CustomerPage {
    public class CreateModel : PageModel {
        private readonly ICustomerBusiness _business;

        public CreateModel() {
            _business ??= new CustomerBusiness();
        }

        public IActionResult OnGet() {
            return Page();
        }

        [BindProperty]
        public Customer Customer { get; set; } = default!;


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            await _business.AddCustomer(Customer);

            return RedirectToPage("./Index");
        }
    }
}
