using Badminton.Business.Interface;
using Badminton.Common;
using Badminton.Data;
using Badminton.Data.DAO;
using Badminton.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
        public Task<IBadmintonResult> AddOrders(Order order);
        public Task<IBadmintonResult> UpdateOrder(Order order);
        public Task<IBadmintonResult> DeleteOrder(int orderId);
        public Task<IBadmintonResult> DeleteOrdersByCustomerId(int orderId);
        public Task<IBadmintonResult> UpdateAmount(int orderId);
        public Task<IBadmintonResult> GetBySearchingNote(string note);
        public Task<IBadmintonResult> GetBySearchingNoteWithCusId(string note, int cusid);
        public Task<IBadmintonResult> Save(Order order);
        public IBadmintonResult Checkout(List<CourtDetail> courtDetailList, int cusID);
    }
    public class OrderBusiness : IOrderBusiness
    {
        private IOrderDetailBusiness _orderDetailBusiness;
        private readonly UnitOfWork _unitOfWork;
        private ICustomerBusiness _customerBusiness;
        public OrderBusiness()
        {
            _customerBusiness ??= new CustomerBusiness();
            _orderDetailBusiness ??= new OrderDetailBusiness();
            _unitOfWork ??= new();
        }

        public async Task<IBadmintonResult> GetAllOrders()
        {
            try
            {
                var orders = await _unitOfWork.OrderRepository.GetAllAsync();
                
                foreach (var order in orders)
                {
                    order.Customer = await AssignCustomerToOrder(order);
                    await UpdateAmount(order.OrderId);
                }
                if (orders == null)
                {
                    return new BadmintonResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }
                return new BadmintonResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, orders.OrderByDescending(o => o.OrderDate).ToList());
            }
            catch (Exception ex)
            {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        private async Task<Customer> AssignCustomerToOrder(Order order)
        {
            var result = await _customerBusiness.GetCustomerById(order.CustomerId);
            var customer = (Customer)result.Data;
            return customer;
        }

        public async Task<IBadmintonResult> GetOrderById(int id)
        {
            try
            {
                var order = await _unitOfWork.OrderRepository.GetByIdAsync(id);
                if (order == null)
                {
                    return new BadmintonResult(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
                }
                order.Customer = await AssignCustomerToOrder(order);
                return new BadmintonResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, order);
            }
            catch (Exception ex)
            {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBadmintonResult> DeleteOrder(int orderId)
        {
            try
            {

                var result = await GetOrderById(orderId);

                if (result.Data == null)
                {
                    return result;
                }
                var check1 = await _orderDetailBusiness.DeleteOrderDetailsByOrderId(orderId);
                if (check1.Status <= 0)
                {
                    return check1;
                }
                var check = _unitOfWork.OrderRepository.Remove((Order)result.Data);

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

        public async Task<IBadmintonResult> AddOrders(Order order)
        {
            try
            {
                var o = await GetOrderById(order.OrderId);

                if (o.Status > 0)
                {
                    return o;
                }
                order.OrderId = 0;
                var check = await _unitOfWork.OrderRepository.CreateAsync(order);
                if (check == 0)
                {
                    return new BadmintonResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
                }
                return new BadmintonResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG, order);

            }
            catch (Exception ex)
            {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBadmintonResult> UpdateOrder(Order order)
        {
            try
            {
                var result = await GetOrderById(order.OrderId);
                if (result.Status <= 0) return result;
                var existingOrder = result.Data as Order;
                existingOrder.OrderDetails = order.OrderDetails;
                existingOrder.CustomerId = order.CustomerId;
                existingOrder.Type = order.Type;
                existingOrder.OrderNotes = order.OrderNotes;
                existingOrder.OrderDate = order.OrderDate;
                var check = await _unitOfWork.OrderRepository.UpdateAsync(existingOrder);

                if (check == 0)
                {
                    return new BadmintonResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
                }

                return new BadmintonResult(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG);
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

                var orders = await _unitOfWork.OrderRepository.GetAllOrdersByCustomerId(customerId);
                if (orders == null)
                {
                    return new BadmintonResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }
                foreach (var order in orders)
                {
                    order.Customer = await AssignCustomerToOrder(order);
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
                    return result;
                }

                List<Order> orders = (List<Order>)result.Data;

                foreach (var order in orders)
                {
                    await DeleteOrder(order.OrderId);
                }

                return new BadmintonResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG);
            }
            catch (Exception ex)
            {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBadmintonResult> UpdateAmount(int orderId)
        {
            try
            {
                var result = await _orderDetailBusiness.GetOrderDetailsByOrderId(orderId);
                double sum = 0;

                List<OrderDetail> orderDetails = (List<OrderDetail>)result.Data;
                orderDetails.ForEach(o => sum += o.Amount);

                var order = (await GetOrderById(orderId)).Data as Order;
                order.TotalAmount = sum;
                _unitOfWork.OrderRepository.PrepareUpdate(order);
                
                var check = await _unitOfWork.OrderRepository.SaveAsync();
                if (check == 0)
                {
                    return new BadmintonResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
                }
                result = await _orderDetailBusiness.GetOrderDetailsByOrderId(orderId);
                return new BadmintonResult(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG);
            }
            catch (Exception ex)
            {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public Task<IBadmintonResult> Save(Order order)
        {
            throw new NotImplementedException();
        }

        public async Task<IBadmintonResult> GetBySearchingNote(string note)
        {
            try
            {
                var result = await GetAllOrders();
                if (result.Status <= 0)
                {
                    return result;
                }
                note ??= string.Empty;
                var allOrders = result.Data as List<Order>;
                var orders = allOrders.Where(d => d.OrderNotes.ToUpper().Contains(note.Trim().ToUpper())).OrderByDescending(o => o.OrderDate).ToList();
                return new BadmintonResult
                {
                    Status = Const.SUCCESS_READ_CODE,
                    Message = Const.SUCCESS_READ_MSG,
                    Data = orders
                };
            }
            catch (Exception ex)
            {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBadmintonResult> GetBySearchingNoteWithCusId(string note, int cusid)
        {
            try
            {
                var result = await GetAllOrdersByCustomerId(cusid);
                if (result.Status <= 0)
                {
                    return result;
                }
                note ??= string.Empty;
                var allOrders = result.Data as List<Order>;
                var orders = allOrders.Where(d => d.OrderNotes.ToUpper().Contains(note.Trim().ToUpper())).OrderByDescending(o => o.OrderDate).ToList();
                return new BadmintonResult
                {
                    Status = Const.SUCCESS_READ_CODE,
                    Message = Const.SUCCESS_READ_MSG,
                    Data = orders
                };
            }
            catch (Exception ex)
            {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
        public IBadmintonResult Checkout(List<CourtDetail> courtDetailList, int cusID)
        {
            try
            {
                var result = _unitOfWork.OrderRepository.Checkout(courtDetailList, cusID, out int orderId);
                if (result <= 0)
                {
                    return new BadmintonResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG, -1);
                }
                return new BadmintonResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG, orderId);
            }
            catch (Exception ex)
            {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
    }
}