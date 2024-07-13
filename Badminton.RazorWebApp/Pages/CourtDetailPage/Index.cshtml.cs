using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Badminton.Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Badminton.Data.Models;

namespace Badminton.RazorWebApp.Pages.CourtDetailPage
{
    public class IndexModel : PageModel
    {
        private readonly ICourtDetailBusiness _courtDetailBusiness;

        public IndexModel(IConfiguration configuration)
        {
            _config = configuration;
            _courtDetailBusiness ??= new CourtDetailBusiness();
        }
        [BindProperty(SupportsGet = true)]    
        public string? Search { get; set; }
        public IList<CourtDetail> CourtDetailList { get;set; } = default!;
        public CourtDetail CourtDetail { get; set; } = default!;
        private readonly IConfiguration _config;
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public async Task OnGetAsync(int newCurPage = 1, string searchKey = @"")
        {
            int pageSize = Int32.TryParse(_config["PageSize"], out int ps) ? ps : 3;
            CurrentPage = newCurPage;
            Search = searchKey;
            if (_courtDetailBusiness != null)
            {
                var courtDetail = await _courtDetailBusiness.GetTopBookedCourt();
                var result = await _courtDetailBusiness.GetAllCourtDetailsIncludeCourt(Search);
                if (result != null && result.Status > 0 && result.Data != null)
                {
                    var list = result.Data as List<CourtDetail>;
                    TotalPages = (int)Math.Ceiling(list.Count / (double)pageSize);
                    CourtDetailList = list.Skip((CurrentPage - 1) * pageSize).Take(pageSize).ToList();
                }

                if (courtDetail != null && courtDetail.Status > 0 && courtDetail.Data != null)
                {
                    CourtDetail = courtDetail.Data as CourtDetail;

                }

            }
        }
    }
}
