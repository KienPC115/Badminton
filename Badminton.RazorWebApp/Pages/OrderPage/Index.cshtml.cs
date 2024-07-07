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
using Badminton.Business.Helpers;
using Badminton.Business.Interface;
namespace Badminton.RazorWebApp.Pages.OrderPage
{
    public class IndexModel : PageModel
    {
        private readonly IOrderBusiness _orderBusiness;
        public IndexModel(IConfiguration configuration)
        {
            _config = configuration;
            _orderBusiness ??= new OrderBusiness();
        }
        public string? NoteSearching { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public List<Order> Orders { get; set; }
        
        private readonly IConfiguration _config;

        public async Task<IActionResult> OnGet(int newCurPage = 1, string note = @"")
        {
            try
            {
                Helpers.GetValueFromSession("cus", out Customer cus, HttpContext);

                int pageSize = int.TryParse(_config["PageSize"], out int ps) ? ps : 3;

                CurrentPage = newCurPage;
                
                NoteSearching = note;

                IBadmintonResult result = new BadmintonResult();
                
                if (cus != null) result = await _orderBusiness.GetBySearchingNoteWithCusId(NoteSearching, cus.CustomerId);

                else result = await _orderBusiness.GetBySearchingNote(NoteSearching);

                if (result.Status <= 0)
                {
                    TempData["message"] = result.Message;
                    return Page();
                }

                var list = result.Data as List<Order>;

                TotalPages = (int)Math.Ceiling(list.Count / (double)pageSize);

                Orders = list.Skip((CurrentPage - 1) * pageSize).Take(pageSize).ToList();

                return Page();
            }
            catch (Exception ez)
            {
                TempData["message"] = ez.Message;
                return Page();
            }
        }
    }
}
