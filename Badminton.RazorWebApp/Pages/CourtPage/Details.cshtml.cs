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
    public class DetailsModel : PageModel
    {
        private readonly ICourtBusiness _courtBusiness;

        public DetailsModel() {
            _courtBusiness ??= new CourtBusiness();
        }

        public Court Court { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id) {
            if (id == null) {
                return NotFound();
            }

            var court = await _courtBusiness.GetCourtById((int)id);
            if (court == null) {
                return NotFound();
            }
            else {
                Court = court.Data as Court;
            }
            return Page();
        }
    }
}
