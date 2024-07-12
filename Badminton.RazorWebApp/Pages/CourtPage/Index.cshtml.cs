using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Badminton.Data.Models;
using Badminton.Business;

namespace Badminton.RazorWebApp.Pages.CourtPage
{
    public class IndexModel : PageModel
    {
        private readonly ICourtBusiness _courtBusiness;

        public IndexModel()
        {
            _courtBusiness ??= new CourtBusiness();
        }

        public IList<Court> Court { get;set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        public async Task OnGetAsync()
        {
            var courtsResult = await _courtBusiness.GetCourtsByKeyword(Search);

            if (courtsResult != null && courtsResult.Status > 0 && courtsResult.Data != null) {
                Court = courtsResult.Data as List<Court>;
            }
        }
    }
}
