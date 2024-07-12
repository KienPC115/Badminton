using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Badminton.Data.Models;
using Badminton.Business;
using Microsoft.AspNetCore.Mvc.Rendering;
using Badminton.Business.Shared;

namespace Badminton.RazorWebApp.Pages.CourtPage
{
    public class IndexModel : PageModel
    {
        private readonly ICourtBusiness _courtBusiness;

        public IndexModel() {
            _courtBusiness ??= new CourtBusiness();
        }
        // Sorting
        public string NameSort { get; set; }
        public string PriceSort { get; set; }
        public string CreatedDateSort { get; set; }
        public string UpdatedDateSort { get; set; }
        // Pagination 
        public int PageSize { get; set; } = 2;
        public int PageNumber { get; set; } = 1;

        public int TotalPages { get; set; }
        // Filter
        [BindProperty(SupportsGet = true)]
        public string SelectedYardType { get; set; } = "All";
        public List<SelectListItem> YardType { get; set; } = CourtShared.YardType().Select(s => new SelectListItem { Value = s, Text = s }).ToList();
        // Current
        public string CurrentSort { get; set; }
        public string CurrentSearch { get; set; }

        public IList<Court> Court { get; set; } = default!;

        public async Task OnGetAsync(string sortOrder, string search = "", int pageNumber = 1) {
            NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            PriceSort = sortOrder == "Price" ? "price_desc" : "Price";
            CreatedDateSort = sortOrder == "CreatedTime" ? "create_desc" : "CreatedTime";
            UpdatedDateSort = sortOrder == "UpdatedTime" ? "update_desc" : "UpdatedTime";

            /*if(SelectedYardType != null)
                YardType.Where(x => x.Text == SelectedYardType).First().Selected = true;*/
            CurrentSort = sortOrder;
            CurrentSearch = search;
            var courtsResult = await _courtBusiness.GetCourtsWithCondition(search, SelectedYardType, sortOrder);  
            
            /*if (courtsResult == null || courtsResult.Status < 0 || courtsResult.Data == null) {
                TempData["message"] = courtsResult.Message;
                return Page();
            }*/

            var list = courtsResult.Data as IList<Court>;

            TotalPages = (int)Math.Ceiling(list.Count / (double)PageSize);
            PageNumber = pageNumber;
            Court = list.Skip((pageNumber - 1) * PageSize).Take(PageSize).ToList();
        }
    }
}
