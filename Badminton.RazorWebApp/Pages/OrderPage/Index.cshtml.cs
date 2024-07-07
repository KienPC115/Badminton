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
            /*if (!(HttpContext.Session.GetInt32("r") == 2 || HttpContext.Session.GetInt32("r") == 3))
            {
                TempData["message"] = "You don't have enough permission. Please try another email.";
                return RedirectToPage("../Index");
            }*/
            try
            {
                if (!int.TryParse(_config["PageSize"].ToString(), out int pageSize))
                {
                    pageSize = 2;
                }
                CurrentPage = newCurPage;
                NoteSearching = note;

                var result = await _orderBusiness.GetBySearchingNote(NoteSearching);
                
                if(result.Status <= 0) TempData["message"] = result.Message;
                
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
