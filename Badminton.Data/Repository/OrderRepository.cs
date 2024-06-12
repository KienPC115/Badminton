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
        public OrderRepository()
        {
            
        }

        public OrderRepository(Net1710_221_8_BadmintonContext context)
        {
            _context = context;
        }
        public async Task<List<Order>> GetAllOrdersByCustomerId(int customerId)
        {
            try
            {
                return await (from o in _context.Orders
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
                _context.Update(order);
                return await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
