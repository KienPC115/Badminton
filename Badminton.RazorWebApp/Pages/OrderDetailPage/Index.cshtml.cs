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
using Microsoft.IdentityModel.Tokens;
using Badminton.Common;

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

        public string Start { get; set; }
        
        public string End { get; set; }

        public int CurrentPage { get; set; } = 1;

        public int TotalPages { get; set; }

        public int OrderID { get; set; }
        public string SearchingString {  get; set; }

        public List<OrderDetail> OrderDetails { get;set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? orderID, string? start, string? end, string? searchString, int newCurPage = 1)
        {
            try
            {
                if (!double.TryParse(start, out double s))s = 0;
                Start = start;

                if (!double.TryParse(end, out double e)) e = double.MaxValue;
                End = end;

                searchString ??= string.Empty;
                SearchingString = searchString;
                
                int pageSize = int.TryParse(_config["PageSize"], out int ps) ? ps : 3;
                
                IBadmintonResult result = new BadmintonResult();
                
                CurrentPage = newCurPage;

                if (orderID == null || orderID == 0) result = await _orderDetailBusiness.GetAllOrderDetails(s, e);

                else
                {
                    OrderID = orderID.Value;
                    result = await _orderDetailBusiness.GetOrderDetailsByOrderId(orderID.Value, s, e);
                }

                if (result.Status <= 0)
                {
                    OrderDetails = new();
                    TempData["message"] = result.Message;
                    return Page();
                }
                
                var list = result.Data as List<OrderDetail>;
                string everything = string.Empty;
                list = list.Where(x =>
                {
                    everything = x.CourtDetail.Court.Description.ToString() + x.CourtDetail.Court.YardType.ToString() + x.CourtDetail.Court.Type.ToString() + x.CourtDetail.Court.Location.ToString() + x.CourtDetail.Court.Name.ToString() + x.CourtDetail.Notes.ToString();
                    return everything.ToUpper().Contains(searchString.Trim().ToUpper());
                }).ToList();

                TotalPages = Helpers.TotalPages(list, pageSize);

                OrderDetails = list.Paging(CurrentPage, pageSize);

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
