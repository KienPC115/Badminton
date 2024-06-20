using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Badminton.Data.Models;
using Badminton.Business;
using Badminton.Data.Base;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;
using System.Xml.Serialization;
using System.IO;
using System.Drawing.Printing;
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
        public string CurrentFilter { get; set; }
        public int TotalPages {  get; set; }
        //public IList<Order> Orders { get;set; } = default!;
        public PaginatedList<Order> Orders { get; set; }

        public async Task OnGetAsync(int? pageIndex, string searchString, string currentFilter)
        {
            try
            {
                #region configuration
                var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory) // Set the base path for configuration file
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
                #endregion
                if (searchString != null) pageIndex = 1;
                else searchString = currentFilter;

                CurrentFilter = searchString;

                var result = await _orderBusiness.GetAllOrders();
                if (!searchString.IsNullOrEmpty()) result = await _orderBusiness.GetBySearchingNote(CurrentFilter);

                if (result.Status > 0)
                {
                    var orders = result.Data as List<Order>;
                    int pageSize = configuration.GetValue<int>("PageSize", 3);
                    TotalPages = (int)Math.Ceiling(orders.Count / (double)pageSize);
                    Orders = await PaginatedList<Order>.CreateAsync(orders, pageIndex ?? 1, pageSize);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
