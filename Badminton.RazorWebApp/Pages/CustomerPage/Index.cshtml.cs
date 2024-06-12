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
    public class IndexModel : PageModel {
        private readonly ICustomerBusiness customerBusiness;

        public IndexModel() {
            customerBusiness ??= new CustomerBusiness();
        }

        public IList<Customer> Customer { get; set; } = default!;

        public async Task OnGetAsync() {
            var result = await customerBusiness.GetAllCustomers();

            if (result != null && result.Status > 0 && result.Data != null) {
                Customer = result.Data as List<Customer>;
            }
        }
    }
}
