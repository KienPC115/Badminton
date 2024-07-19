using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Badminton.Business;
using Badminton.Business.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Badminton.Data.Models;

namespace Badminton.RazorWebApp.Pages.CourtDetailPage
{
    public class DeleteModel : CustomPage
    {
        private readonly CourtDetailBusiness _courtDetailBusiness;

        public DeleteModel()
        {
            _courtDetailBusiness = new CourtDetailBusiness();
        }

        [BindProperty]
      public CourtDetail CourtDetail { get; set; } = default!;

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

            var courtdetail = await _courtDetailBusiness.GetCourtDetail(id.Value);

            if (courtdetail == null)
            {
                return NotFound();
            }
            else 
            {
                CourtDetail = courtdetail.Data as CourtDetail;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            var type = this.GetType();
            if (id == null)
            {
                return NotFound();
            }
            var courtdetail = await _courtDetailBusiness.GetCourtDetail(id.Value);

            if (courtdetail != null)
            {
                CourtDetail = courtdetail.Data as CourtDetail;
                CourtDetail.Status = CourtDetailShared.DELETE;
                await _courtDetailBusiness.UpdateCourtDetail(CourtDetail.CourtDetailId, CourtDetail,
             CourtDetailShared.DELETE);
            }
            return RedirectToPage("./Index");
        }
    }
}
