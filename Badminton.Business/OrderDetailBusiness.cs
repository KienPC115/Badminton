using Badminton.Business.Interface;
using Badminton.Business.Shared;
using Badminton.Common;
using Badminton.Data;
using Badminton.Data.Models;

namespace Badminton.Business
{
    public interface IOrderDetailBusiness
    {
        public Task<IBadmintonResult> GetAllOrderDetails(double start = 0, double end = double.MaxValue);
        public Task<IBadmintonResult> GetOrderDetailsByOrderId(int orderId, double start = 0, double end = double.MaxValue);
        public Task<IBadmintonResult> GetOrderDetailsByCourtDetailId(int courtDetailId);
        public Task<IBadmintonResult> GetOrderDetailById(int orderDetailId);
        public Task<IBadmintonResult> UpdateOrderDetail(OrderDetail result);
        public Task<IBadmintonResult> AddOrderDetail(OrderDetail result);
        public Task<IBadmintonResult> DeleteOrderDetail(int orderDetailId);
        public Task<IBadmintonResult> DeleteOrderDetailsByOrderId(int orderId);
        public Task<IBadmintonResult> DeleteOrderDetailsByCourtDetailId(int courtDetailId);
    }
    public class OrderDetailBusiness : IOrderDetailBusiness
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly CourtDetailBusiness _courtDetailBusiness;
        public OrderDetailBusiness()
        {
            _courtDetailBusiness = new CourtDetailBusiness();
            _unitOfWork ??= new();
        }
        public async Task<IBadmintonResult> AddOrderDetail(OrderDetail orderDetail)
        {
            try
            {
                var result = await GetOrderDetailById(orderDetail.OrderDetailId);
                if (result.Status > 0)
                {
                    return new BadmintonResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
                }
                orderDetail.OrderDetailId = 0;
                orderDetail.Amount = _unitOfWork.CourtDetailRepository.GetById(orderDetail.CourtDetailId).Price;
                var check = await _unitOfWork.OrderDetailRepository.CreateAsync(orderDetail);
                if (check == 0)
                {
                    return new BadmintonResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
                }
                
                result = await _courtDetailBusiness.GetCourtDetail(orderDetail.CourtDetailId);
                var courtDetail = result.Data as CourtDetail;
                courtDetail.Status = "Booked";
                result = await _courtDetailBusiness.UpdateCourtDetail(courtDetail.CourtDetailId, courtDetail, CourtDetailShared.UPDATE);
                if (result.Status <= 0)
                {
                    return result;
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

                orderDetails.ForEach(od => _unitOfWork.OrderDetailRepository.Remove(od));
                result = await GetOrderDetailsByOrderId(orderId);

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
                var result = await GetOrderDetailById(orderDetailId);

                if (result.Data == null)
                {
                    return result;
                }

                OrderDetail orderDetail = (OrderDetail)result.Data;

                result = await _courtDetailBusiness.GetCourtDetail(orderDetail.CourtDetailId);
                var oldCourtDetail = result.Data as CourtDetail;
                oldCourtDetail.Status = "Available";
                result = await _courtDetailBusiness.UpdateCourtDetail(oldCourtDetail.CourtDetailId, oldCourtDetail, CourtDetailShared.UPDATE);

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

        public async Task<IBadmintonResult> GetAllOrderDetails(double start = 0, double end = double.MaxValue)
        {
            try
            {
                var orderDetails = _unitOfWork.OrderDetailRepository.GetAllAsync().Result.Where(o => o.Amount >= start && o.Amount <= end).ToList();
                if (orderDetails == null)
                {
                    return new BadmintonResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }
                orderDetails.ForEach(od =>
                {
                    od.Order = _unitOfWork.OrderRepository.GetById(od.OrderId);
                    od.Order.Customer = _unitOfWork.CustomerRepository.GetById(od.Order.CustomerId);
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

        public async Task<IBadmintonResult> GetOrderDetailsByOrderId(int orderId, double start = 0, double end = double.MaxValue)
        {
            try
            {
                var result = _unitOfWork.OrderDetailRepository.GetAll()
                    .Where(o => o.OrderId.Equals(orderId) && o.Amount >= start && o.Amount <= end).ToList();
                if (result.Count() <= 0 || result == null)
                {
                    return new BadmintonResult(Const.FAIL_READ_CODE, Const.WARNING_NO_DATA__MSG);
                }
                result.ForEach(od =>
                {
                    od.Order = _unitOfWork.OrderRepository.GetById(od.OrderId);
                    od.Order.Customer = _unitOfWork.CustomerRepository.GetById(od.Order.CustomerId);
                    od.CourtDetail = _unitOfWork.CourtDetailRepository.GetById(od.CourtDetailId);
                    od.CourtDetail.Court = _unitOfWork.CourtRepository.GetById(od.CourtDetailId);
                });
                return new BadmintonResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result);
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
                od.OrderId = orderDetail.OrderId;
                od.Amount = _unitOfWork.CourtDetailRepository.GetById(od.CourtDetailId).Price;
                od.Order = null;
                od.CourtDetail = null;
                var check = await _unitOfWork.OrderDetailRepository.UpdateAsync(od);
                if (check == 0)
                {
                    return new BadmintonResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
                }

                result = await _courtDetailBusiness.GetCourtDetail(orderDetail.CourtDetailId);
                var newCourtDetail = result.Data as CourtDetail;
                newCourtDetail.Status = "Booked";
                result = await _courtDetailBusiness.UpdateCourtDetail(newCourtDetail.CourtDetailId, newCourtDetail, CourtDetailShared.UPDATE);

                if (result.Status <= 0)
                {
                    return result;
                }

                return new BadmintonResult(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG, check);
            }
            catch (Exception ex)
            {
                return new BadmintonResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
    }
}