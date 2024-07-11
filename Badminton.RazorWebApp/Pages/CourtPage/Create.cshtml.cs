using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Badminton.Data.Models;
using Badminton.Business.Shared;
using Badminton.Business;

namespace Badminton.RazorWebApp.Pages.CourtPage
{
    public class CreateModel : PageModel
    {
        private readonly ICourtBusiness _courtBusiness;

        public CreateModel() {
            _courtBusiness ??= new CourtBusiness();
        }

        public IActionResult OnGet() {
            Status = CourtShared.Status().Select(s => new SelectListItem { Value = s, Text = s, Selected = s == "Available" ? true : false }).ToList();
            YardType = CourtShared.YardType().Select(s => new SelectListItem { Value = s, Text = s, Selected = s == "PVC carpet" ? true : false }).ToList();
            Type = CourtShared.Type().Select(s => new SelectListItem { Value = s, Text = s, Selected = s == "Single" ? true : false }).ToList();
            Location = CourtShared.Location().Select(s => new SelectListItem { Value = s, Text = s, Selected = s == "Location A" ? true : false }).ToList();
            SpaceType = CourtShared.SpaceType().Select(s => new SelectListItem { Value = s, Text = s, Selected = s == "Indoor" ? true : false }).ToList();
            return Page();
        }

        [BindProperty]
        public Court Court { get; set; } = default!;

        public List<SelectListItem> Status { get; set; }
        public List<SelectListItem> YardType { get; set; }
        public List<SelectListItem> Type { get; set; }
        public List<SelectListItem> Location { get; set; }
        public List<SelectListItem> SpaceType { get; set; }



        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            await _courtBusiness.AddCourt(Court);

            return RedirectToPage("./Index");
        }
    }
}
