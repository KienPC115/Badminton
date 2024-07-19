using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Badminton.Data.Base;
using Badminton.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Badminton.Data.Repository {
    public class CustomerRepository : GenericRepository<Customer> {
        public CustomerRepository() { }

        public async Task<List<Customer>> GetAllCustomerBySearchKeyAsync(string search)
        {
            return await _context.Customers.Where(cus => cus.Name.ToLower().Contains(search.ToLower())
                                                  || cus.Address.ToLower().Contains(search.ToLower())
                                                  || cus.Phone.ToLower().Contains(search.ToLower())).ToListAsync();
        }

        public Customer GetCustomerByEmail(string email) => _context.Customers.FirstOrDefault(c => c.Email.Equals(email));
    }
}
