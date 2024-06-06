using Badminton.Business.Interface;
using Badminton.Common;
using Badminton.Data;
using Badminton.Data.DAO;
using Badminton.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
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
                var od = await GetOrderDetailById(result.OrderDetailId);
                if (od.Status > 0)
                {
                    return new BadmintonResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
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

                if (result.Status <= 0) return result;

                var orderDetails = result.Data as List<OrderDetail>;

                orderDetails.ForEach(od => _unitOfWork.OrderDetailRepository.PrepareUpdate(od));
                var check = _unitOfWork.OrderDetailRepository.Save();

                if (check < orderDetails.Count) return new BadmintonResult(Const.FAIL_DELETE_CODE, Const.FAIL_DELETE_MSG);
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
                var result = await GetOrderDetailsByOrderId(orderId);
                if (result.Status <= 0) return result;

                var orderDetails = result.Data as List<OrderDetail>;

                orderDetails.ForEach(od => _unitOfWork.OrderDetailRepository.PrepareRemove(od));
                var check = _unitOfWork.OrderDetailRepository.Save();

                if (check < orderDetails.Count) return new BadmintonResult(Const.FAIL_DELETE_CODE, Const.FAIL_DELETE_MSG);
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
                    return od;
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
                orderDetails.ForEach(od =>
                {
                    od.Order = _unitOfWork.OrderRepository.GetById(od.OrderId);
                    od.CourtDetail = _unitOfWork.CourtDetailRepository.GetById(od.CourtDetailId);
                    od.CourtDetail.Court = _unitOfWork.CourtRepository.GetById(od.CourtDetailId);
                });

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
                var result = await GetAllOrderDetails();
                if (result.Status <= 0)
                {
                    return new BadmintonResult(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
                }
                List<OrderDetail> orderDetails = (result.Data as List<OrderDetail>).Where(od => od.OrderId == orderId).ToList();
                return new BadmintonResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, orderDetails);
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
                var result = await GetAllOrderDetails();
                if (result.Status <= 0)
                {
                    return new BadmintonResult(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
                }
                var orderDetail = (result.Data as List<OrderDetail>).FirstOrDefault(od => od.OrderDetailId == orderDetailId);
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

        public async Task<IBadmintonResult> GetOrderDetailsByCourtDetailId(int courtDetailId)
        {
            try
            {
                var result = await GetAllOrderDetails();
                if (result.Status <= 0)
                {
                    return new BadmintonResult(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
                }
                var orderDetail = (result.Data as List<OrderDetail>).FirstOrDefault(od => od.CourtDetailId == courtDetailId);
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
                var result = await GetOrderDetailById(orderDetail.OrderDetailId);
                if (result.Status <= 0)
                {
                    return result;
                }
                var od = result.Data as OrderDetail;
                od.CourtDetailId = orderDetail.CourtDetailId;
                od.OrderDetailId = orderDetail.OrderDetailId;
                od.Amount = orderDetail.Amount;
                var check = await _unitOfWork.OrderDetailRepository.UpdateAsync(od);
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