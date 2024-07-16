using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Badminton.Data.Models;
using Badminton.Business;
using Badminton.Business.Shared;

namespace Badminton.RazorWebApp.Pages.OrderDetailPage
{
    public class EditModel : PageModel
    {
        private readonly IOrderBusiness _orderBusiness;
        private readonly ICourtDetailBusiness _courtDetailBusiness;
        private readonly IOrderDetailBusiness _orderDetailBusiness;
        private readonly ICourtBusiness _courtBusiness;
        List<string> courtDetailStatus = CourtDetailShared.Status();

        public EditModel()
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
            
            CourtDetails = CourtDetails.Where(cd => cd.Status == courtDetailStatus[0]).ToList();
            
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

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _courtDetailBusiness.GetCourtDetail(OrderDetail.CourtDetailId);
            var courtDetail = result.Data as CourtDetail;
            courtDetail.Status = courtDetailStatus[1];
            result = await _courtDetailBusiness.UpdateCourtDetail(courtDetail.CourtDetailId, courtDetail, CourtDetailShared.UPDATE);
            result = await _orderDetailBusiness.UpdateOrderDetail(OrderDetail);
            if (result.Status <= 0)
            {
                return Page();
            }
            return RedirectToPage("./Index", new { orderID = OrderDetail.OrderId });
        }
    }
}
