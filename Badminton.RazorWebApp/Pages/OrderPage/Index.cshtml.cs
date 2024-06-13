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
    public class IndexModel : PageModel
    {
        private readonly IOrderBusiness _orderBusiness;
        public IndexModel()
        {
            _orderBusiness ??= new OrderBusiness();
        }
        [BindProperty]
        public string Key { get; set; }
        public IList<Order> Order { get;set; } = default!;

        public async Task OnGetAsync()
        {
            var result = await _orderBusiness.GetAllOrders();
            if (result.Status > 0)
            {
                Order = result.Data as List<Order>;
            }
        }
        public async Task OnPostAsync()
        {
            var result = await _orderBusiness.GetBySearchingNote(Key);
            if (result.Status > 0)
            {
                Order = result.Data as List<Order>;
            }
        }
    }
}
