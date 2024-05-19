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
    public interface IOrderDetailBunsiness
    {
        public Task<IBadmintonResult> GetAllOrderDetails();
        public Task<IBadmintonResult> GetOrderDetailsByOrderId(int orderId);
        public Task<IBadmintonResult> GetOrderDetailsByCourtDetailId(int courtDetailId);
        public Task<IBadmintonResult> GetOrderDetail(int orderDetailId);
        public Task<IBadmintonResult> UpdateOrderDetail(IBadmintonResult result);
        public Task<IBadmintonResult> AddOrderDetail(IBadmintonResult result);
        public Task<IBadmintonResult> DeleteOrderDetail(int orderDetailId);
        public Task<IBadmintonResult> DeleteOrderDetailsByOrderId(int orderId);
        public Task<IBadmintonResult> DeleteOrderDetailsByCourtDetailId(int courtDetailId);
    }
    public class OrderDetailBusiness : IOrderDetailBunsiness
    {
        private Net1710_221_8_BadmintonContext _context;
        private IOrderDetailBunsiness _orderDetailBusiness = new OrderDetailBusiness();

        public async Task<IBadmintonResult> AddOrderDetail(IBadmintonResult result)
        {
            try
            {
                _context = new();

                var orderDetail = result.Data as OrderDetail;
                if (orderDetail == null)
                {
                    return new BadmintonResult();
                }

                var od = await GetOrderDetail(orderDetail.OrderId);
                if (od.Data != null)
                {
                    return new BadmintonResult();
                }

                _context.OrderDetails.Add(orderDetail);
                await _context.SaveChangesAsync();
                return new BadmintonResult(1, "Success");
            }
            catch (Exception ex)
            {
                return new BadmintonResult(-1, ex.Message);
            }
        }

        public async Task<IBadmintonResult> DeleteOrderDetailsByCourtDetailId(int courtDetailId)
        {
            try
            {
                var result = await _orderDetailBusiness.GetOrderDetailsByCourtDetailId(courtDetailId);

                if (result.Data == null)
                {
                    return new BadmintonResult();
                }

                List<OrderDetail> orderDetails = (List<OrderDetail>)result.Data;

                _context = new();

                foreach (var orderDetail in orderDetails)
                {
                    await _orderDetailBusiness.DeleteOrderDetail(orderDetail.OrderDetailId);
                }

                await _context.SaveChangesAsync();
                return new BadmintonResult(1, "Success");
            }
            catch (Exception ex)
            {
                return new BadmintonResult(-1, ex.Message);
            }
        }

        public async Task<IBadmintonResult> DeleteOrderDetailsByOrderId(int orderId)
        {
            try
            {
                var result = await _orderDetailBusiness.GetOrderDetailsByOrderId(orderId);

                if (result.Data == null)
                {
                    return new BadmintonResult(1, "Success");
                }

                List<OrderDetail> orderDetails = (List<OrderDetail>)result.Data;

                _context = new();

                foreach (var orderDetail in orderDetails)
                {
                    await _orderDetailBusiness.DeleteOrderDetail(orderDetail.OrderDetailId);
                }

                await _context.SaveChangesAsync();
                return new BadmintonResult(1, "Success");
            }
            catch (Exception ex)
            {
                return new BadmintonResult(-1, ex.Message);
            }
        }

        public async Task<IBadmintonResult> DeleteOrderDetail(int orderDetailId)
        {
            try
            {
                var od = await GetOrderDetail(orderDetailId);

                if (od.Data == null)
                {
                    return new BadmintonResult(1, "Success");
                }

                OrderDetail orderDetail = (OrderDetail)od.Data;

                _context = new();
                _context.OrderDetails.Remove(orderDetail);
                await _context.SaveChangesAsync();

                return new BadmintonResult(1, "Success");
            }
            catch (Exception ex)
            {
                return new BadmintonResult(-1, ex.Message);
            }
        }

        public async Task<IBadmintonResult> GetAllOrderDetails()
        {
            try
            {
                _context = new();

                var orderDetails = await _context.OrderDetails.ToListAsync();

                return new BadmintonResult(1, "Success", orderDetails);
            }
            catch (Exception ex)
            {
                return new BadmintonResult(-1, ex.Message);
            }
        }

        public async Task<IBadmintonResult> GetOrderDetailsByOrderId(int orderId)
        {
            try
            {
                _context = new();

                var orderDetail = await(from od in _context.OrderDetails
                                        where od.OrderId == orderId
                                        select od).ToListAsync();

                return new BadmintonResult(1, "Success", orderDetail);
            }
            catch (Exception ex)
            {
                return new BadmintonResult(-1, ex.Message);
            }
        }

        public async Task<IBadmintonResult> GetOrderDetail(int orderDetailId)
        {
            try
            {
                _context = new();

                var orderDetail = await _context.OrderDetails.FirstOrDefaultAsync(o => o.OrderDetailId == orderDetailId);

                if (orderDetail == null)
                {
                    return new BadmintonResult(-1, "No exist", null);
                }

                return new BadmintonResult(1, "Success", orderDetail);
            }
            catch (Exception ex)
            {
                return new BadmintonResult(-1, ex.Message);
            }
        }

        public async Task<IBadmintonResult> GetOrderDetailsByCourtDetailId(int courtDetailId)
        {
            try
            {
                _context = new();

                var orderDetail = await (from od in _context.OrderDetails
                                         where od.CourtDetailId == courtDetailId
                                         select od).ToListAsync();

                return new BadmintonResult(1, "Success", orderDetail);
            }
            catch (Exception ex)
            {
                return new BadmintonResult(-1, ex.Message);
            }
        }

        public async Task<IBadmintonResult> UpdateOrderDetail(IBadmintonResult result)
        {
            try
            {
                _context = new();

                if (result.Data == null)
                {
                    return new BadmintonResult();
                }

                var orderDetail = (OrderDetail)result.Data;
                var od = await GetOrderDetail(orderDetail.OrderDetailId);

                if (od.Data == null)
                {
                    return new BadmintonResult();
                }

                _context.OrderDetails.Update(orderDetail);
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
