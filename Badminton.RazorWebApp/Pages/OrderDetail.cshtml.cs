using Badminton.Business;
using Badminton.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Badminton.RazorWebApp.Pages
{
    public class OrderDetailModel : PageModel
    {
        private readonly IOrderBusiness _orderBusiness = new OrderBusiness();
        private readonly ICourtDetailBusiness _courtDetailBusiness = new CourtDetailBusiness();
        private readonly IOrderDetailBunsiness _orderDetailBunsiness = new OrderDetailBusiness();
        private readonly ICourtBusiness _courtBusiness = new CourtBusiness();

        [BindProperty]
        public string Message { get; set; }

        [BindProperty]
        public OrderDetail OrderDetail { get; set; } = new OrderDetail();

        public List<Order> Orders { get; set; } = new List<Order>();

        public List<Court> Courts { get; set; } = new List<Court>();

        public List<CourtDetail> CourtDetails { get; set; } = new List<CourtDetail>();

        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        public void OnGet()
        {
            CourtDetails = this.GetCourtDetails();
            OrderDetails = this.GetOrderDetails();
            Orders = this.GetOrders();
            Courts = this.GetCourts();
        }

        private List<Court> GetCourts()
        {
            var result = _courtBusiness.GetAllCourts();
            if (result.Result.Data == null)
            {
                return new();
            }
            var courts = (List<Court>)result.Result.Data;
            return courts;
        }

        private List<Order> GetOrders()
        {
            var result = _orderBusiness.GetAllOrders();
            if (result.Result.Data == null)
            {
                return new();
            }
            var orders = (List<Order>)result.Result.Data;
            return orders;
        }

        private List<OrderDetail> GetOrderDetails()
        {
            var result = _orderDetailBunsiness.GetAllOrderDetails();
            if (result.Result.Data == null)
            {
                return new();
            }
            var orderDetails = (List<OrderDetail>)result.Result.Data;
            foreach (var orderDetail in orderDetails)
            {
                var courtDetail = (CourtDetail)_courtDetailBusiness.GetCourtDetail(orderDetail.OrderDetailId).Result.Data;

                var court = (Court)_courtBusiness.GetCourtById(courtDetail.CourtId).Result.Data;

                var order = (Order)_orderBusiness.GetOrderById(orderDetail.OrderId).Result.Data;
                
                courtDetail.Court = court;
                orderDetail.CourtDetail = courtDetail;
                orderDetail.Order = order;
            }
            return orderDetails;
        }

        private List<CourtDetail> GetCourtDetails()
        {
            var result = _courtDetailBusiness.GetAllCourtDetails();
            if (result.Result.Data == null)
            {
                return new();
            }
            var courtDetails = (List<CourtDetail>)result.Result.Data;
            return courtDetails;
        }

        public void OnPost() { this.SaveOrderDetail(); }

        private void SaveOrderDetail()
        {
            var result = _orderDetailBunsiness.AddOrderDetail(this.OrderDetail);
            if (result != null)
            {
                this.Message = result.Result.Message;
                OnGet();
            }
            else
            {
                this.Message = "Error System";
            }
        }
    }
}