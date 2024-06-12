using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Badminton.Data.Models;
using Badminton.Business;

namespace Badminton.RazorWebApp.Pages.OrderPage
{
    public class DetailsModel : PageModel
    {
        private readonly IOrderBusiness _orderBusiness;
        public DetailsModel()
        {
            _orderBusiness ??= new OrderBusiness();
        }

        public Order Order { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var result = await _orderBusiness.GetOrderById(id.Value);
            if (result.Status < 0)
            {
                return NotFound();
            }
            Order = (result.Data as Order);

            if (Order == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
