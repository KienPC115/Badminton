using Badminton.Data.Base;
using Badminton.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Badminton.Data.DAO
{
    public class OrderDAO : BaseDAO<Order>
    {
        public OrderDAO() { }

        public async Task<List<Order>> GetAllOrdersByCustomerId(int customerId)
        {
            try
            {
                return await (from o in _dbSet
                              where o.CustomerId == customerId
                              select o).ToListAsync();
            }catch (Exception)
            {
                throw;
            }
        }
    }
}
