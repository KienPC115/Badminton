using Badminton.Common;
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
    public class OrderDetailRepository : GenericRepository<OrderDetail>
    {
        public List<OrderDetail> GetAllOrderDetails() => _context.OrderDetails.Include(od => od.Order).ThenInclude(o => o.Customer).Include(c => c.CourtDetail).ThenInclude(c => c.Court).ToList();

        public OrderDetail? GetOrderDetail(int id) => _context.OrderDetails.Include(od => od.Order).ThenInclude(o => o.Customer).Include(c => c.CourtDetail).ThenInclude(c => c.Court).FirstOrDefault(od => od.OrderDetailId == id);

        public int UpdateOrderDetail(OrderDetail newOD)
        {
            try
            {
                var oldOD = _context.OrderDetails.FirstOrDefault(c => newOD.OrderDetailId == c.OrderDetailId);

                if (oldOD.CourtDetailId != newOD.CourtDetailId)
                {
                    var oldCD = _context.CourtDetails.FirstOrDefault(cd => cd.CourtDetailId == oldOD.CourtDetailId);

                    var newCD = _context.CourtDetails.FirstOrDefault(cd => cd.CourtDetailId == newOD.CourtDetailId);

                    var temp = newCD.Status;
                    newCD.Status = oldCD.Status;
                    oldCD.Status = temp;

                    _context.CourtDetails.Update(oldCD);
                    _context.CourtDetails.Update(newCD);

                    newOD.Amount = newCD.Price;
                }

                oldOD.CopyValues(newOD);
                
                _context.OrderDetails.Update(oldOD);

                var order = _context.Orders.Include(o => o.OrderDetails).FirstOrDefault(o => o.OrderId == oldOD.OrderId);
                order.TotalAmount = order.OrderDetails.Sum(o => o.Amount);

                _context.Orders.Update(order);

                return _context.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
