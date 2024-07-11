﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Badminton.Data.Models;
using Badminton.Business.Shared;
using Badminton.Business;

namespace Badminton.RazorWebApp.Pages.CourtPage
{
    public class EditModel : PageModel
    {
        private readonly ICourtBusiness _courtBusiness;

        public EditModel() {
            _courtBusiness ??= new CourtBusiness();
        }

        [BindProperty]
        public Court Court { get; set; } = default!;
        public List<SelectListItem> Status { get; set; }
        public List<SelectListItem> YardType { get; set; }
        public List<SelectListItem> Type { get; set; }
        public List<SelectListItem> Location { get; set; }
        public List<SelectListItem> SpaceType { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id) {
            if (id == null) {
                return NotFound();
            }

            var court = await _courtBusiness.GetCourtById((int)id);
            if (court == null) {
                return NotFound();
            }

            Status = CourtShared.Status().Select(s => new SelectListItem { Value = s, Text = s, Selected = s == "Available" ? true : false }).ToList();
            YardType = CourtShared.YardType().Select(s => new SelectListItem { Value = s, Text = s, Selected = s == "PVC carpet" ? true : false }).ToList();
            Type = CourtShared.Type().Select(s => new SelectListItem { Value = s, Text = s, Selected = s == "Single" ? true : false }).ToList();
            Location = CourtShared.Location().Select(s => new SelectListItem { Value = s, Text = s, Selected = s == "Location A" ? true : false }).ToList();
            SpaceType = CourtShared.SpaceType().Select(s => new SelectListItem { Value = s, Text = s, Selected = s == "Indoor" ? true : false }).ToList();
            // Load dropdown
            // ViewData["CustomerId"] = new SelectList(_context.Customers,"CustomerId", "Address","selectedValue")
            Court = court.Data as Court;

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            try {
                await _courtBusiness.UpdateCourt(Court.CourtId, Court);
            }
            catch (DbUpdateConcurrencyException) {
                if (!(await CourtExistsAsync(Court.CourtId))) {
                    return NotFound();
                }
                else {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private async Task<bool> CourtExistsAsync(int id) {
            var result = await _courtBusiness.GetCourtById((int)id);
            return result.Data != null;
        }
    }
}
