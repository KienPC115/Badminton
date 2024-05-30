using System.Collections.Generic;
using Badminton.Business;
using Badminton.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Badminton.RazorWebApp.Pages {
    public class CustomerModel : PageModel {
        private readonly ICustomerBusiness _customerBusiness = new CustomerBusiness();

        public string Message { get; set; } = default!;

        [BindProperty]
        public Customer Customer { get; set; } = default!;
        public List<Customer> Customers { get; set; } = new List<Customer>();


        public void OnPost() {
            this.SaveCustomer();
        }

        public void OnDelete() {
        }


        private List<Customer> GetCustomers() {
            var CustomerResult = _customerBusiness.GetAllCustomers();

            if (CustomerResult.Status > 0 && CustomerResult.Result.Data != null) {
                var Customers = (List<Customer>)CustomerResult.Result.Data;
                return Customers;
            }
            return new List<Customer>();
        }

        private void SaveCustomer() {
            var CustomerResult = _customerBusiness.AddCustomer(this.Customer);

            if (CustomerResult != null) {
                this.Message = CustomerResult.Result.Message!;
            } else {
                this.Message = "Error system";
            }
        }
    }
}