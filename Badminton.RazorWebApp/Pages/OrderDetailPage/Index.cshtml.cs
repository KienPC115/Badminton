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
    public class IndexModel : PageModel
    {
        private readonly IOrderBusiness _orderBusiness;
        private readonly ICourtDetailBusiness _courtDetailBusiness;
        private readonly IOrderDetailBusiness _orderDetailBusiness;
        private readonly ICourtBusiness _courtBusiness;

        public IndexModel()
        {
            _orderBusiness ??= new OrderBusiness();
            _orderDetailBusiness ??= new OrderDetailBusiness();
            _courtDetailBusiness ??= new CourtDetailBusiness();
            _courtBusiness ??= new CourtBusiness();
        }

        public IList<OrderDetail> OrderDetail { get;set; } = default!;

        public async Task OnGetAsync()
        {
            var result = await _orderDetailBusiness.GetAllOrderDetails();
            OrderDetail = result.Data as List<OrderDetail>;
        }
    }
}
