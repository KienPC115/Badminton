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
        public List<Order> GetAllOrdersByCustomerId(int customerId)
        {
            return _context.Orders.Where(o => o.CustomerId == customerId).Include(o => o.Customer).ToList();
        }

        public List<Order> GetAllOrders()
        {
            return _context.Orders
                         .Include(o => o.Customer).ToList();
        }
        
        public Order GetOrderById(int id)
        {
            return _context.Orders.FirstOrDefault(c => c.OrderId == id);
        }

        public Order Checkout(List<CourtDetail> courtDetailsList, int customerId, string note, string type)
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
                        OrderNotes = note,
                        TotalAmount = totalAmount,
                        Type = type,
                        Email = "",
                        PhoneOrder = "",
                        OrderStatus = "Pending",
                        ModifiedDate = DateTime.Now,
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
                    return order;
                }
                catch (Exception)
                {
                    check = -1;
                    transaction.Rollback();
                }
                return null;
            }
        }
    }
}
