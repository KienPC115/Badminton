using Badminton.Business.Interface;
using Badminton.Common;
using Badminton.Data.DAO;
using Badminton.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
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
        private readonly OrderDAO _DAO;
        public OrderBusiness()
        {
            _DAO = new OrderDAO();
        }
        private IOrderDetailBunsiness _orderDetailBusiness = new OrderDetailBusiness();
        IOrderBusiness _orderBusiness = new OrderBusiness();

        public async Task<IBadmintonResult> GetAllOrders()
        {
            try
            {

                var orders = await _DAO.GetAllAsync();

                if (orders == null)
                {
                    return new BadmintonResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }
                return new BadmintonResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, orders);
            }
            catch (Exception ex)
            {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBadmintonResult> GetOrderById(int id)
        {
            try
            {
                var order = await _DAO.GetByIdAsync(id);
                if (order == null)
                {
                    return new BadmintonResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }
                return new BadmintonResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, order);
            }
            catch (Exception ex)
            {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBadmintonResult> DeleteOrders(int orderId)
        {
            try
            {

                var o = await GetOrderById(orderId);

                if (o.Data == null)
                {
                    return new BadmintonResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }

                var check = await _DAO.RemoveAsync((Order)o.Data);

                await _orderDetailBusiness.DeleteOrderDetailsByOrderId(orderId);

                if (!check)
                {
                    return new BadmintonResult(Const.FAIL_DELETE_CODE, Const.FAIL_DELETE_MSG);
                }

                return new BadmintonResult(Const.SUCCESS_DELETE_CODE, Const.SUCCESS_DELETE_MSG);
            }
            catch (Exception ex)
            {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBadmintonResult> AddOrders(IBadmintonResult result)
        {
            try
            {

                var order = result.Data as Order;

                if (order == null)
                {
                    return new BadmintonResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }

                var o = await GetOrderById(order.OrderId);

                if (o.Data != null)
                {
                    return new BadmintonResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }

                var check = await _DAO.CreateAsync(order);
                if (check == 0)
                {
                    return new BadmintonResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
                }
                return new BadmintonResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, order);

            }
            catch (Exception ex)
            {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBadmintonResult> UpdateOrders(IBadmintonResult result)
        {
            try
            {

                var order = result.Data as Order;

                if (order == null)
                {
                    return new BadmintonResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }

                var o = await GetOrderById(order.OrderId);

                if (o.Data == null)
                {
                    return new BadmintonResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }

                var check = await _DAO.UpdateAsync(order);

                if (check == 0)
                {
                    return new BadmintonResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
                }

                return new BadmintonResult(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG, order);
            }
            catch (Exception ex)
            {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBadmintonResult> GetAllOrdersByCustomerId(int customerId)
        {
            try
            {

                var orders = await _DAO.GetAllOrdersByCustomerId(customerId);
                if (orders == null)
                {
                    return new BadmintonResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }
                return new BadmintonResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, orders);
            }
            catch (Exception ex)
            {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBadmintonResult> DeleteOrdersByCustomerId(int customerId)
        {
            try
            {
                var result = await GetAllOrdersByCustomerId(customerId);

                if (result.Data == null)
                {
                    return new BadmintonResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG);
                }

                List<Order> orders = (List<Order>)result.Data;

                foreach (var order in orders)
                {
                    await _orderBusiness.DeleteOrders(order.OrderId);
                }

                return new BadmintonResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG);
            }
            catch (Exception ex)
            {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
    }
}
