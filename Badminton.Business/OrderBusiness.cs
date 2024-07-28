using Badminton.Business.Interface;
using Badminton.Business.Shared;
using Badminton.Common;
using Badminton.Data;
using Badminton.Data.DAO;
using Badminton.Data.Models;
using Microsoft.AspNetCore.Http;
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
        public Task<IBadmintonResult> GetBySearchingNote(string note, double start, double end);
        public Task<IBadmintonResult> GetBySearchingNoteWithCusId(string note, int cusid, double start, double end);
        public Task<IBadmintonResult> Save(Order order);
        public IBadmintonResult Checkout(List<CourtDetail> courtDetailList, int cusID, string note, string type);
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
                var orders = _unitOfWork.OrderRepository.GetAllOrders();
                
                if (orders == null)
                {
                    return new BadmintonResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }
                return new BadmintonResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, orders.OrderByDescending(o => o.OrderId).ToList());
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
                var order = _unitOfWork.OrderRepository.GetOrderById(id);
                if (order == null)
                {
                    return new BadmintonResult(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
                }
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
                var result = await _unitOfWork.OrderRepository.GetByIdAsync(order.OrderId);
                
                if (result == null) return new BadmintonResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);

                result.CopyValues(order);
                
                var check = await _unitOfWork.OrderRepository.UpdateAsync(result);

                if (check == 0) return new BadmintonResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);

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

                var orders = _unitOfWork.OrderRepository.GetAllOrdersByCustomerId(customerId);
                if (orders == null) return new BadmintonResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
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

        public async Task<IBadmintonResult> GetBySearchingNote(string note, double start, double end)
        {
            try
            {
                var allOrders = _unitOfWork.OrderRepository.GetAllOrders();
                
                note ??= string.Empty;

                var orders = allOrders.Where(d => d.TotalAmount >= start && d.TotalAmount <= end).OrderByDescending(o => o.OrderDate).ToList();

                var words = note.Split('~');

                foreach (var word in words)
                {
                    orders = orders.Where(o => (o.OrderDate.ToString()+ "~" +o.Type+ "~" +o.OrderNotes+ "~" +o.Customer.Name+ "~" +o.Customer.Address+ "~" +o.Customer.Email+ "~" +o.Customer.DateOfBirth.ToString()+ "~" +o.Customer.Phone+ "~" +o.TotalAmount + "~" + o.Email + "~" + o.PhoneOrder+ "~" + o.OrderStatus).ToUpper().Trim().Contains(word.ToUpper().Trim())).ToList();
                }

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

        public async Task<IBadmintonResult> GetBySearchingNoteWithCusId(string note, int cusid, double start, double end)
        {
            try
            {
                note ??= string.Empty;

                var allOrders =  _unitOfWork.OrderRepository.GetAllOrdersByCustomerId(cusid);

                var orders = allOrders.Where(d => d.TotalAmount >= start && d.TotalAmount <= end).OrderByDescending(o => o.OrderDate).ToList();

                var words = note.Split('~');

                foreach (var word in words)
                {
                    orders = orders.Where(o => (o.OrderDate.ToString() + "~" + o.Type + "~" + o.OrderNotes + "~" + o.Customer.Name + "~" + o.Customer.Address + "~" + o.Customer.Email + "~" + o.Customer.DateOfBirth.ToString() + "~" + o.Customer.Phone + "~" + o.TotalAmount + "~" + o.Email + "~" + o.PhoneOrder + "~" + o.OrderStatus).ToUpper().Trim().Contains(word.ToUpper().Trim())).ToList();

                }

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

        public string CreatePaymentUrl(Order model)
        {
            try
            {
                var tick = DateTime.Now.Ticks.ToString();
                var vnpay = new VnPayLibrary();
                vnpay.AddRequestData("vnp_Version", "2.1.0");
                vnpay.AddRequestData("vnp_Command", "Pay");
                vnpay.AddRequestData("vnp_TmnCode", "NJJ0R8FS");
                vnpay.AddRequestData("vnp_Amount", (model.TotalAmount * 100).ToString());
                vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
                vnpay.AddRequestData("vnp_CurrCode", "VND");
                vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
                vnpay.AddRequestData("vnp_Locale", "vn");
                vnpay.AddRequestData("vnp_OrderInfo", "Pay Order:" + model.OrderId);
                vnpay.AddRequestData("vnp_OrderType", "other"); // default value: other
                vnpay.AddRequestData("vnp_ReturnUrl", "https://localhost:7287/PaymentPage");
                vnpay.AddRequestData("vnp_TxnRef", tick);

                var paymentUrl = vnpay.CreateRequestUrl("https://sandbox.vnpayment.vn/paymentv2/vpcpay.html", "BYKJBHPPZKQMKBIBGGXIYKWYFAYSJXCW");
                return paymentUrl;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public IBadmintonResult Checkout(List<CourtDetail> courtDetailList, int cusID, string note, string type)
        {
            try
            {
                var result = _unitOfWork.OrderRepository.Checkout(courtDetailList, cusID, note, type);
                var listType = OrderShared.Type();
                if (result == null)
                {
                    return new BadmintonResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
                }
                var paymentUrl = string.Empty;
                if (type.Equals(listType[1]))
                {
                    paymentUrl = CreatePaymentUrl(result);
                }
                return new BadmintonResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG, paymentUrl = paymentUrl + " " + result.OrderId);
            }
            catch (Exception ex)
            {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
    }
}