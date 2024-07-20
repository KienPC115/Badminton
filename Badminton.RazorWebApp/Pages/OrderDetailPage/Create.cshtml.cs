using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Badminton.Data.Models;
using Badminton.Business;
using Badminton.Business.Shared;
using Badminton.Common;

namespace Badminton.RazorWebApp.Pages.OrderDetailPage
{
    public class CreateModel : PageModel
    {
        private readonly IOrderBusiness _orderBusiness;
        private readonly ICourtDetailBusiness _courtDetailBusiness;
        private readonly IOrderDetailBusiness _orderDetailBusiness;
        private readonly ICourtBusiness _courtBusiness;

        public CreateModel()
        {
            _orderBusiness ??= new OrderBusiness();   
            _orderDetailBusiness ??= new OrderDetailBusiness();
            _courtDetailBusiness ??= new CourtDetailBusiness();
            _courtBusiness ??= new CourtBusiness();
        }
        public List<Order> Orders { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
        public List<Court> Courts { get; set; }
        public List<CourtDetail> CourtDetails { get; set; }
        public IActionResult OnGet()
        {
            if (Helpers.GetValueFromSession("cus", out Customer cus, HttpContext) && cus.Name.Equals("admin") && cus.Email.Equals("admin@example.com"))
            {
                Orders = _orderBusiness.GetAllOrders().Result.Data as List<Order>;
                CourtDetails = _courtDetailBusiness.GetAllCourtDetails().Result.Data as List<CourtDetail>;
                var status = CourtDetailShared.Status();
                CourtDetails = CourtDetails.Where(cd => cd.Status.Equals(status[0])).ToList();
                CourtDetails.ForEach(cd => cd.Court = _courtBusiness.GetCourtById(cd.CourtId).Result.Data as Court);
                return Page();
            }
            return RedirectToPage("../Index");
        }
        [BindProperty]
        public OrderDetail OrderDetail { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            var result = await _orderDetailBusiness.AddOrderDetail(OrderDetail);
            if (result.Status <= 0)
            {
                return Page();
            }
            return RedirectToPage("./Index", new { orderID = OrderDetail.OrderId });
        }
    }
}
