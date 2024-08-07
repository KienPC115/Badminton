﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Badminton.Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Badminton.Data.Models;

namespace Badminton.RazorWebApp.Pages.CourtDetailPage
{
    public class DetailsModel : CustomPage
    {
        private readonly ICourtDetailBusiness _courtDetailBusiness;
        public DetailsModel()
        {
            _courtDetailBusiness = new CourtDetailBusiness();
        }

      public CourtDetail CourtDetail { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            IsAdmin = CheckAdmin();

            if (!IsAdmin) {
                TempData["message"] = "You don't have enough permission.";
                return RedirectToPage("./Index");
            }
            if (id == null )
            {
                return NotFound();
            }
            var courtDetail = await _courtDetailBusiness.GetCourtDetail(id.Value);

            if (courtDetail.Data == null)
            {
                return NotFound();
            }
            else 
            {
                CourtDetail = courtDetail.Data as CourtDetail;
            }
            return Page();
        }
    }
}
