using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Badminton.Business;
using Badminton.Business.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Badminton.Data.Models;

namespace Badminton.RazorWebApp.Pages.CourtDetailPage
{
    public class CreateModel : PageModel
    {
        private readonly ICourtDetailBusiness _courtDetailBusiness;
        private readonly ICourtBusiness _courtBusiness;

        public CreateModel()
        {
            _courtDetailBusiness = new CourtDetailBusiness();
            _courtBusiness = new CourtBusiness();
        }

        public async Task<IActionResult> OnGet()
        {
            Slot = CourtDetailShared.Slot();
            Status = CourtDetailShared.Status();
            var result = await _courtBusiness.GetCourtsByStatus(CourtShared.Status()[0]);
            Courts = result.Data as List<Court>;
            return Page();
        }

        [BindProperty]
        public CourtDetail CourtDetail { get; set; } = default!;

        public List<string> Slot { get; set; } = default!;
        public List<Court> Courts { get; set; } = default!;

        public List<string> Status { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            await _courtDetailBusiness.AddCourtDetail(CourtDetail);

            return RedirectToPage("./Index");
        }
    }
}
