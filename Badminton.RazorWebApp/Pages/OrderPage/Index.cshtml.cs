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
using Badminton.Common;
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
        public string Start {  get; set; }
        public string End { get; set; }
        public List<Order> Orders { get; set; }
        
        private readonly IConfiguration _config;

        public async Task<IActionResult> OnGet(string? note, string? start, string? end, int newCurPage = 1)
        {
            if (Helpers.GetValueFromSession("cus", out Customer cus, HttpContext))
            {
                try
                {
                    if (!double.TryParse(start, out double s)) s = 0;
                    Start = start;

                    if (!double.TryParse(end, out double e)) e = double.MaxValue;
                    End = end;

                    int pageSize = int.TryParse(_config["PageSize"], out int ps) ? ps : 3;

                    CurrentPage = newCurPage;

                    NoteSearching = note;

                    IBadmintonResult result = new BadmintonResult();

                    if (cus.Name.Equals("admin") && cus.Email.Equals("admin@example.com")) result = await _orderBusiness.GetBySearchingNote(NoteSearching, s, e);

                    else result = await _orderBusiness.GetBySearchingNoteWithCusId(NoteSearching, cus.CustomerId, s, e);

                    if (result.Status <= 0)
                    {
                        TempData["message"] = result.Message;
                        return Page();
                    }

                    var list = result.Data as List<Order>;

                    TotalPages = Helpers.TotalPages(list, pageSize);

                    Orders = list.Paging(CurrentPage, pageSize);
                }
                catch (Exception ez)
                {
                    TempData["message"] = ez.Message;
                    return Page();
                }
            }
            return Page();
        }
    }
}
