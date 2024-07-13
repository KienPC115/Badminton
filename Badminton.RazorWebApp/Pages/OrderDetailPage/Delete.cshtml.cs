using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Badminton.Data.Models;
using Badminton.Business;

namespace Badminton.RazorWebApp.Pages.OrderDetailPage
{
    public class DeleteModel : PageModel
    {
        private readonly IOrderBusiness _orderBusiness;
        private readonly ICourtDetailBusiness _courtDetailBusiness;
        private readonly IOrderDetailBusiness _orderDetailBusiness;
        private readonly ICourtBusiness _courtBusiness;

        public DeleteModel()
        {
            _orderBusiness ??= new OrderBusiness();
            _orderDetailBusiness ??= new OrderDetailBusiness();
            _courtDetailBusiness ??= new CourtDetailBusiness();
            _courtBusiness ??= new CourtBusiness();
        }
        public List<Order> Orders { get; set; }
        public List<Court> Courts { get; set; }
        public List<CourtDetail> CourtDetails { get; set; }
        [BindProperty]
        public OrderDetail OrderDetail { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            Orders = _orderBusiness.GetAllOrders().Result.Data as List<Order>;
            CourtDetails = _courtDetailBusiness.GetAllCourtDetails().Result.Data as List<CourtDetail>;
            CourtDetails.ForEach(cd => cd.Court = _courtBusiness.GetCourtById(cd.CourtId).Result.Data as Court);
            var result = await _orderDetailBusiness.GetOrderDetailById(id.Value);
            if (result.Status < 0)
            {
                return NotFound();
            }
            OrderDetail = (result.Data as OrderDetail);

            if (OrderDetail == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            var orderdetail = await _orderDetailBusiness.DeleteOrderDetail(id.Value);

            if (orderdetail.Status <= 0)
            {
                return Page();
            }

            return RedirectToPage("../OrderPage/Index");
        }
    }
}
