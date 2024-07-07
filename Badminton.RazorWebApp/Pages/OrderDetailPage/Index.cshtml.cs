using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Badminton.Data.Models;
using Badminton.Business;
using Badminton.Business.Interface;

namespace Badminton.RazorWebApp.Pages.OrderDetailPage
{
    public class IndexModel : PageModel
    {
        private readonly IOrderBusiness _orderBusiness;
        private readonly ICourtDetailBusiness _courtDetailBusiness;
        private readonly IOrderDetailBusiness _orderDetailBusiness;
        private readonly ICourtBusiness _courtBusiness;
        private readonly IConfiguration _config;

        public IndexModel(IConfiguration configuration)
        {
            _config = configuration;
            _orderBusiness ??= new OrderBusiness();
            _orderDetailBusiness ??= new OrderDetailBusiness();
            _courtDetailBusiness ??= new CourtDetailBusiness();
            _courtBusiness ??= new CourtBusiness();
        }

        public double Start { get; set; }
        
        public double End { get; set; }

        public int CurrentPage { get; set; } = 1;

        public int TotalPages { get; set; }

        public int OrderID { get; set; }

        public List<OrderDetail> OrderDetails { get;set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? orderID, double start = 0, double end = double.MaxValue, int newCurPage = 1)
        {
            try
            {
                int pageSize = int.TryParse(_config["PageSize"], out int ps) ? ps : 3;
                IBadmintonResult result = new BadmintonResult();
                Start = start;
                End = end;
                CurrentPage = newCurPage;

                if (orderID == null || orderID == 0) result = await _orderDetailBusiness.GetAllOrderDetails(start, end);

                else
                {
                    OrderID = orderID.Value;
                    result = await _orderDetailBusiness.GetOrderDetailsByOrderId(orderID.Value, start, end);
                }

                if (result.Status <= 0)
                {
                    OrderDetails = new();
                    TempData["message"] = result.Message;
                    return Page();
                }

                var list = result.Data as List<OrderDetail>;

                TotalPages = (int)Math.Ceiling(list.Count / (double)pageSize);

                OrderDetails = list.Skip((CurrentPage - 1) * pageSize).Take(pageSize).ToList();

                return Page();
            }
            catch (Exception ex)
            {
                TempData["message"] = ex.Message;
                return Page();
            }
        }
    }
}
