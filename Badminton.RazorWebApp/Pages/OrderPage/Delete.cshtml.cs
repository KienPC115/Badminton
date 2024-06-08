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
    public class DeleteModel : PageModel
    {
        private readonly IOrderBusiness _orderBusiness;
        public DeleteModel()
        {
            _orderBusiness ??= new OrderBusiness();
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var result = await _orderBusiness.GetOrderById(id.Value);
            var order = result.Data as Order;
            if (order != null)
            {
                Order = order;
                result = await _orderBusiness.DeleteOrder(id.Value);
                if (result.Status <= 0)
                {
                    return NotFound();
                }
            }

            return RedirectToPage("./Index");
        }
    }
}
