using Badminton.Data.Base;
using Badminton.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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
        public int Checkout(List<CourtDetail> courtDetailsList, int customerId)
        {
            int check = -1;
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {

                    double totalAmount = courtDetailsList.Sum(c => c.Price);
                    
                    var order = new Order
                    {
                        CustomerId = customerId,
                        OrderDate = DateTime.Now,
                        OrderNotes = "Thanh toan",
                        TotalAmount = totalAmount,
                        Type = "Cash Payment"
                    };
                    _context.Orders.Add(order);
                    check = _context.SaveChanges();

                    if (check <= 0)
                    {
                        transaction.Rollback();
                    }

                    List<CourtDetail> courtDetailsExisting = new List<CourtDetail>();
                    courtDetailsList.ForEach(o =>
                    {
                        courtDetailsExisting.Add(_context.CourtDetails.FirstOrDefault(c => c.CourtDetailId == o.CourtDetailId));
                        
                        _context.OrderDetails.Add(new OrderDetail
                        {
                            Amount = o.Price,
                            CourtDetailId = o.CourtDetailId,
                            OrderId = order.OrderId,
                        });
                    });
                    courtDetailsExisting.ForEach(c =>
                    {
                        c.BookingCount += 1;
                        c.Status = "Booked";
                    });
                    _context.CourtDetails.UpdateRange(courtDetailsExisting);
                    check = _context.SaveChanges();
                    if (check <= 0)
                    {
                        transaction.Rollback();
                    }
                    else
                    {
                        transaction.Commit();
                    }
                    check = order.OrderId;
                }
                catch (Exception)
                {
                    check = -1;
                    transaction.Rollback();
                }
            }
            return check;
        }
    }
}
