using Badminton.Business;
using Badminton.Data.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
namespace Badminton.RazorWebApp.Pages
{
    public class OrderDetailModel : PageModel
    {
        private readonly IOrderBusiness _orderBusiness = new OrderBusiness();
        private readonly ICourtDetailBusiness _courtDetailBusiness = new CourtDetailBusiness();
        private readonly IOrderDetailBusiness _orderDetailBusiness = new OrderDetailBusiness();
        private readonly ICourtBusiness _courtBusiness = new CourtBusiness();

        [TempData]
        public string Message { get; set; }

        [BindProperty]
        public OrderDetail OrderDetail { get; set; } = new OrderDetail();

        public List<Order> Orders { get; set; } = new List<Order>();

        public List<Court> Courts { get; set; } = new List<Court>();

        public List<CourtDetail> CourtDetails { get; set; } = new List<CourtDetail>();

        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        public async Task OnGetAsync()
        {
            CourtDetails = await GetCourtDetailsAsync();
            OrderDetails = await GetOrderDetailsAsync();
            Orders = await GetOrdersAsync();
            Courts = await GetCourtsAsync();
        }

        private async Task<List<Court>> GetCourtsAsync()
        {
            var result = await _courtBusiness.GetAllCourts();
            if (result.Data == null)
            {
                return new List<Court>();
            }
            var courts = (List<Court>)result.Data;
            foreach (var court in courts)
            {
                var temp = (await GetCourtDetailsAsync()).Where(cd => cd.CourtId == court.CourtId);
                court.CourtDetails = temp.ToList();
            }
            return courts;
        }

        private async Task<List<Order>> GetOrdersAsync()
        {
            var result = await _orderBusiness.GetAllOrders();
            if (result.Data == null)
            {
                return new List<Order>();
            }
            var orders = (List<Order>)result.Data;
            return orders;
        }

        private async Task<List<OrderDetail>> GetOrderDetailsAsync()
        {
            var result = await _orderDetailBusiness.GetAllOrderDetails();
            if (result.Data == null)
            {
                return new List<OrderDetail>();
            }
            var orderDetails = (List<OrderDetail>)result.Data;
            foreach (var orderDetail in orderDetails)
            {
                var courtDetailResult = await _courtDetailBusiness.GetCourtDetail(orderDetail.CourtDetailId);
                if (courtDetailResult.Data == null) continue;
                var courtDetail = (CourtDetail)courtDetailResult.Data;

                var courtResult = await _courtBusiness.GetCourtById(courtDetail.CourtId);
                if (courtResult.Data == null) continue;
                var court = (Court)courtResult.Data;

                var orderResult = await _orderBusiness.GetOrderById(orderDetail.OrderId);
                if (orderResult.Data == null) continue;
                var order = (Order)orderResult.Data;

                courtDetail.Court = court;
                orderDetail.CourtDetail = courtDetail;
                orderDetail.Order = order;
            }
            return orderDetails;
        }

        private async Task<List<CourtDetail>> GetCourtDetailsAsync()
        {
            var result = await _courtDetailBusiness.GetAllCourtDetails();
            if (result.Data == null)
            {
                return new List<CourtDetail>();
            }
            var courtDetails = (List<CourtDetail>)result.Data;
            return courtDetails;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await SaveOrderDetailAsync();
            return RedirectToPage("/OrderDetail");
        }

        private async Task SaveOrderDetailAsync()
        {
            var courtDetailResult = await _courtDetailBusiness.GetCourtDetail(OrderDetail.CourtDetailId);
            if (courtDetailResult.Data == null)
            {
                Message = "Court detail not found";
                return;
            }
            var courtDetail = courtDetailResult.Data as CourtDetail;
            OrderDetail.Amount = courtDetail.Price;

            var result = await _orderDetailBusiness.AddOrderDetail(OrderDetail);

            if (result != null)
            {
                Message = result.Message;
            }
            else
            {
                Message = "Error System";
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var check = await _orderDetailBusiness.GetOrderDetailById(id);
            if (check.Status <= 0)
            {
                Message = check.Message;
                return RedirectToPage("/OrderDetail");
            }
            var result = await _orderDetailBusiness.DeleteOrderDetail(id);
            if (result != null)
            {
                Message = result.Message;
            }
            else
            {
                Message = "Error System";
            }
            return RedirectToPage("/OrderDetail");
        }

        public async Task<IActionResult> OnPostPutAsync(OrderDetail orderDetail)
        {
            var courtDetailResult = await _courtDetailBusiness.GetCourtDetail(orderDetail.CourtDetailId);
            if (courtDetailResult.Data == null)
            {
                Message = "Court detail not found";
                return RedirectToPage("/OrderDetail");
            }
            var courtDetail = courtDetailResult.Data as CourtDetail;
            orderDetail.Amount = courtDetail.Price;
            var result = await _orderDetailBusiness.UpdateOrderDetail(orderDetail);
            if (result != null)
            {
                Message = result.Message;
            }
            else
            {
                Message = "Error System";
            }
            return RedirectToPage("/OrderDetail");
        }
    }
}