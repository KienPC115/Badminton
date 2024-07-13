using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Badminton.Business;
using Badminton.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Badminton.RazorWebApp.Pages.CustomerPage {
    public class IndexModel : PageModel {
        private readonly ICustomerBusiness customerBusiness;

        public IndexModel(IConfiguration config)
        {
            _config = config;
            customerBusiness ??= new CustomerBusiness();
        }

        private readonly IConfiguration _config;
        public string Search { get; set; }
        public int TotalPages  { get; set; }
        public int CurrentPage = 1;
        public IList<Customer> CustomerList { get; set; } = default!;

        public async Task OnGetAsync(int newCurPage = 1, string searchKey = @"")
        {
            int pageSize = int.TryParse(_config["PageSize"], out int ps) ? ps : 3;
            Search = searchKey;
            CurrentPage = newCurPage;
            var result = !string.IsNullOrEmpty(searchKey)
                ? await customerBusiness.GetAllCustomersWithSearchKey(Search)
                : await customerBusiness.GetAllCustomers();
            if (result.Status <= 0)
            {
                TempData["message"] = result.Message;
                return;
            }

            var list = result.Data as List<Customer>;

            TotalPages = (int)Math.Ceiling(list.Count / (double)pageSize);
            CustomerList = list.Skip((CurrentPage - 1) * pageSize).Take(pageSize).ToList();
            
        }
    }
}
