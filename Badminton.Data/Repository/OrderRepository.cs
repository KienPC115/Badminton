using Badminton.Data.Base;
using Badminton.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Badminton.Data.Repository
{
    public class OrderRepository : GenericRepository<Order>
    {
        public async Task<List<Order>> GetAllOrdersByCustomerId(int customerId)
        {
            try
            {
                return await (from o in _dbSet
                              where o.CustomerId == customerId
                              select o).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> UpdateOrder(Order order)
        {
            try
            {
                _dbSet.Update(order);
                return await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
