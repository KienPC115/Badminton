using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Badminton.Data.Models;

namespace Badminton.RazorWebApp.Pages.CourtDetailPage
{
    public class DetailsModel : PageModel
    {
        private readonly Badminton.Data.Models.Net1710_221_8_BadmintonContext _context;

        public DetailsModel(Badminton.Data.Models.Net1710_221_8_BadmintonContext context)
        {
            _context = context;
        }

      public CourtDetail CourtDetail { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.CourtDetails == null)
            {
                return NotFound();
            }

            var courtdetail = await _context.CourtDetails.FirstOrDefaultAsync(m => m.CourtDetailId == id);
            if (courtdetail == null)
            {
                return NotFound();
            }
            else 
            {
                CourtDetail = courtdetail;
            }
            return Page();
        }
    }
}
