using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Badminton.Business;
using Badminton.Business.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Badminton.Data.Models;

namespace Badminton.RazorWebApp.Pages.CourtDetailPage
{
    public class EditModel : CustomPage
    {
        private readonly CourtDetailBusiness _courtDetailBusiness;
        private readonly CourtBusiness _courtBusiness;

        public EditModel()
        {
            _courtDetailBusiness = new CourtDetailBusiness();
            _courtBusiness = new CourtBusiness();
        }

        [BindProperty]
        public CourtDetail CourtDetail { get; set; } = default!;

        public List<string> Slot { get; set; } = default!;
        public List<string> Status { get; set; } = default!;
        public List<Court> Courts { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            IsAdmin = CheckAdmin();

            if (!IsAdmin) {
                TempData["message"] = "You don't have enough permission.";
                return RedirectToPage("./Index");
            }
            if (id == null)
            {
                return NotFound();
            }

            Slot = CourtDetailShared.Slot();
            Status = CourtDetailShared.Status();
            Courts = (await _courtBusiness.GetAllCourts()).Data as List<Court>;
            var courtdetail = await _courtDetailBusiness.GetCourtDetail(id.Value);
            if (courtdetail.Status < 0 || courtdetail.Data == null)
            {
                return NotFound();
            }
            CourtDetail = courtdetail.Data as CourtDetail;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            try
            {
                await _courtDetailBusiness.UpdateCourtDetail(CourtDetail.CourtDetailId, CourtDetail, CourtDetailShared.UPDATE);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!(await CourtDetailExists(CourtDetail.CourtDetailId)))
                {
                    return NotFound();
                }
                else
                {
                    throw;  
                }
            }

            return RedirectToPage("./Index");
        }

        private async Task<bool> CourtDetailExists(int id)
        {
            var result = await _courtDetailBusiness.GetCourtDetail(id);
            return result.Data != null;
        }
    }
}
