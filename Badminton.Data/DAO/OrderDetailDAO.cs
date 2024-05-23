﻿using Badminton.Data.Base;
using Badminton.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Badminton.Data.DAO
{
    public class OrderDetailDAO : BaseDAO<OrderDetail>
    {
        public OrderDetailDAO()
        {
        }

        public async Task<List<OrderDetail>> GetOrderDetailsByCourtDetailId(int courtDetailId)
        {
            try
            {
                return await (from od in _dbSet
                              where od.CourtDetailId == courtDetailId
                              select od).ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<OrderDetail>> GetOrderDetailsByOrderId(int orderId)
        {
            try
            {
                return await (from od in _dbSet
                              where od.OrderId == orderId
                              select od).ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
