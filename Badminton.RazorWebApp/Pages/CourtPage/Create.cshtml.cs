using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Badminton.Data.Models;
using Badminton.Business;
using Badminton.Business.Shared;

namespace Badminton.RazorWebApp.Pages.CourtPage
{
    public class CreateModel : PageModel
    {
        private readonly ICourtBusiness _courtBusiness;

        public CreateModel()
        {
            _courtBusiness ??= new CourtBusiness();
        }

        public IActionResult OnGet()
        {
            Status = CourtShared.Status();
            return Page();
        }

        [BindProperty]
        public Court Court { get; set; } = default!;

        public List<string> Status { get; set; } = default!;


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) {
                return Page();
            }

            await _courtBusiness.AddCourt(Court);

            return RedirectToPage("./Index");
        }
    }
}
