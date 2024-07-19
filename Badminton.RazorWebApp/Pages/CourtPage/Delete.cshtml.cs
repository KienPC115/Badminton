using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Badminton.Data.Models;
using Badminton.Business;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace Badminton.RazorWebApp.Pages.CourtPage
{
    public class DeleteModel : CustomPage
    {
        private readonly ICourtBusiness _courtBusiness;
        public DeleteModel() {
            _courtBusiness ??= new CourtBusiness();
        }

        [BindProperty]
        public Court Court { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id) {
            IsAdmin = CheckAdmin();

            if (!IsAdmin) {
                TempData["message"] = "You don't have enough permission.";
                return RedirectToPage("./Index");
            }

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

        public async Task<IActionResult> OnPostAsync(int? id) {
            if (id == null) {
                return NotFound();
            }

            var court = await _courtBusiness.DeleteCourt((int)id);
            if (court.Status <= 0) {
                TempData["message"] = court.Message;
            }

            return RedirectToPage("./Index");
        }
    }
}
