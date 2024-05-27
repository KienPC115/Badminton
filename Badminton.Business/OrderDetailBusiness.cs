using Badminton.Business.Interface;
using Badminton.Common;
using Badminton.Data;
using Badminton.Data.DAO;
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
        public Task<IBadmintonResult> GetOrderDetailById(int orderDetailId);
        public Task<IBadmintonResult> UpdateOrderDetail(OrderDetail result);
        public Task<IBadmintonResult> AddOrderDetail(OrderDetail result);
        public Task<IBadmintonResult> DeleteOrderDetail(int orderDetailId);
        public Task<IBadmintonResult> DeleteOrderDetailsByOrderId(int orderId);
        public Task<IBadmintonResult> DeleteOrderDetailsByCourtDetailId(int courtDetailId);
    }
    public class OrderDetailBusiness : IOrderDetailBunsiness
    {
        private readonly UnitOfWork _unitOfWork;
        public OrderDetailBusiness()
        {
            _unitOfWork ??= new();
        }
        public async Task<IBadmintonResult> AddOrderDetail(OrderDetail result)
        {
            try
            {
                var od = await GetOrderDetailById(result.OrderId);
                if (od.Data != null)
                {
                    return new BadmintonResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }

                var check = await _unitOfWork.OrderDetailRepository.CreateAsync(result);
                if (check == 0)
                {
                    return new BadmintonResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
                }
                return new BadmintonResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG);
            }
            catch (Exception ex)
            {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBadmintonResult> DeleteOrderDetailsByCourtDetailId(int courtDetailId)
        {
            try
            {
                var result = await GetOrderDetailsByCourtDetailId(courtDetailId);

                if (result.Data == null)
                {
                    return new BadmintonResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }

                List<OrderDetail> orderDetails = (List<OrderDetail>)result.Data;

                foreach (var orderDetail in orderDetails)
                {
                    await DeleteOrderDetail(orderDetail.OrderDetailId);
                }
                return new BadmintonResult(Const.SUCCESS_DELETE_CODE, Const.SUCCESS_DELETE_MSG);
            }
            catch (Exception ex)
            {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBadmintonResult> DeleteOrderDetailsByOrderId(int orderId)
        {
            try
            {
                var orderDetails = await _unitOfWork.OrderDetailRepository.GetOrderDetailsByOrderId(orderId);

                if (orderDetails == null)
                {
                    return new BadmintonResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }

                foreach (var orderDetail in orderDetails)
                {
                    await DeleteOrderDetail(orderDetail.OrderDetailId);
                }
                return new BadmintonResult(Const.SUCCESS_DELETE_CODE, Const.SUCCESS_DELETE_MSG);
            }
            catch (Exception ex)
            {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBadmintonResult> DeleteOrderDetail(int orderDetailId)
        {
            try
            {
                var od = await GetOrderDetailById(orderDetailId);

                if (od.Data == null)
                {
                    return new BadmintonResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }

                OrderDetail orderDetail = (OrderDetail)od.Data;
                var check = await _unitOfWork.OrderDetailRepository.RemoveAsync(orderDetail);
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

        public async Task<IBadmintonResult> GetAllOrderDetails()
        {
            try
            {
                var orderDetails = await _unitOfWork.OrderDetailRepository.GetAllAsync();
                if (orderDetails == null)
                {
                    return new BadmintonResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }
                return new BadmintonResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, orderDetails);
            }
            catch (Exception ex)
            {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBadmintonResult> GetOrderDetailsByOrderId(int orderId)
        {
            try
            {
                var orderDetail = await _unitOfWork.OrderDetailRepository.GetOrderDetailsByOrderId(orderId);
                if (orderDetail == null)
                {
                    return new BadmintonResult(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
                }
                return new BadmintonResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, orderDetail);
            }
            catch (Exception ex)
            {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBadmintonResult> GetOrderDetailById(int orderDetailId)
        {
            try
            {
                var orderDetail = await _unitOfWork.OrderDetailRepository.GetByIdAsync(orderDetailId);
                if (orderDetail == null)
                {
                    return new BadmintonResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }
                return new BadmintonResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, orderDetail);
            }
            catch (Exception ex)
            {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBadmintonResult> GetOrderDetailsByCourtDetailId(int courtDetailId)
        {
            try
            {
                var orderDetail = await _unitOfWork.OrderDetailRepository.GetOrderDetailsByCourtDetailId(courtDetailId);
                if (orderDetail == null)
                {
                    return new BadmintonResult(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
                }
                return new BadmintonResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, orderDetail);
            }
            catch (Exception ex)
            {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBadmintonResult> UpdateOrderDetail(OrderDetail orderDetail)
        {
            try
            {
                var od = await GetOrderDetailById(orderDetail.OrderDetailId);
                if (od.Data == null)
                {
                    return new BadmintonResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }
                var check = await _unitOfWork.OrderDetailRepository.UpdateAsync(orderDetail);
                if (check == 0)
                {
                    return new BadmintonResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
                }
                return new BadmintonResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, check);
            }
            catch (Exception ex)
            {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
    }
}
