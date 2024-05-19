using Badminton.Business.Interface;
using Badminton.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Badminton.Business
{
    public interface IOrderBusiness
    {
        public Task<IBadmintonResult> GetAllOrders();
        public Task<IBadmintonResult> GetAllOrdersByCustomerId(int id);
        public Task<IBadmintonResult> GetOrderById(int orderId);
        public Task<IBadmintonResult> AddOrders(IBadmintonResult order);
        public Task<IBadmintonResult> UpdateOrders(IBadmintonResult order);
        public Task<IBadmintonResult> DeleteOrders(int orderId);
        public Task<IBadmintonResult> DeleteOrdersByCustomerId(int orderId);
    }
    public class OrderBusiness : IOrderBusiness
    {
        IOrderBusiness _orderBusiness = new OrderBusiness();
        //Do not use read-only because it is necessary to use "_context = new()" so that the data is refreshed every time the database is called
        private Net1710_221_8_BadmintonContext _context;
        private IOrderDetailBunsiness _orderDetailBusiness = new OrderDetailBusiness();

        public async Task<IBadmintonResult> GetAllOrders()
        {
            try
            {
                _context = new();
                var orders = await _context.Orders.ToListAsync();
                return new BadmintonResult(1, "Success", orders);
            }
            catch (Exception ex)
            {
                return new BadmintonResult(-1, ex.Message);
            }
        }

        public async Task<IBadmintonResult> GetOrderById(int id)
        {
            try
            {
                _context = new();
                var order = await _context.Orders.FirstOrDefaultAsync();
                if (order != null)
                {
                    return new BadmintonResult(1, "Success", order);
                }
                return new BadmintonResult(1, "Success", null);
            }
            catch (Exception ex)
            {
                return new BadmintonResult(-1, ex.Message);
            }
        }

        public async Task<IBadmintonResult> DeleteOrders(int orderId)
        {
            try
            {
                _context = new();
                var o = await GetOrderById(orderId);

                if (o.Data == null)
                {
                    return new BadmintonResult(1, "Success");
                }

                _context.Orders.Remove((Order)o.Data);

                await _orderDetailBusiness.DeleteOrderDetailsByOrderId(orderId);

                await _context.SaveChangesAsync();

                return new BadmintonResult(1, "Success");
            }
            catch (Exception ex)
            {
                return new BadmintonResult(-1, ex.Message);
            }
        }

        public async Task<IBadmintonResult> AddOrders(IBadmintonResult result)
        {
            try
            {
                _context = new();
                var order = result.Data as Order;
                var o = await GetOrderById(order.OrderId);

                if (o.Data != null)
                {
                    return new BadmintonResult();
                }

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                return new BadmintonResult(1, "Success");
            }
            catch (Exception ex)
            {
                return new BadmintonResult(-1, ex.Message);
            }
        }

        public async Task<IBadmintonResult> UpdateOrders(IBadmintonResult result)
        {
            try
            {
                _context = new();
                var order = result.Data as Order;
                var o = await GetOrderById(order.OrderId);
                if (o.Data == null)
                {
                    return new BadmintonResult();
                }
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
                return new BadmintonResult(1, "Success");
            }
            catch (Exception ex)
            {
                return new BadmintonResult(-1, ex.Message);
            }
        }

        public async Task<IBadmintonResult> GetAllOrdersByCustomerId(int customerId)
        {
            try
            {
                _context = new();
                var orders = await (from o in _context.Orders
                                   where o.CustomerId == customerId
                                   select o).ToListAsync();
                return new BadmintonResult(1, "Success", orders);
            }
            catch (Exception ex)
            {
                return new BadmintonResult(-1, ex.Message);
            }
        }

        public async Task<IBadmintonResult> DeleteOrdersByCustomerId(int customerId)
        {
            try
            {
                _context = new();
                var result = await _orderBusiness.GetAllOrdersByCustomerId(customerId);

                if (result.Data == null)
                {
                    return new BadmintonResult(1, "Success");
                }

                List<Order> orders = (List<Order>)result.Data;

                foreach (var order in orders)
                {
                    await _orderBusiness.DeleteOrders(order.OrderId);
                }

                await _context.SaveChangesAsync();

                return new BadmintonResult(1, "Success");
            }
            catch (Exception ex)
            {
                return new BadmintonResult(-1, ex.Message);
            }
        }
    }
}
