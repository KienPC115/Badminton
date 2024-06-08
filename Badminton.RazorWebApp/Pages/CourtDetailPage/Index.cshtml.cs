using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Badminton.Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Badminton.Data.Models;

namespace Badminton.RazorWebApp.Pages.CourtDetailPage
{
    public class IndexModel : PageModel
    {
        private readonly ICourtDetailBusiness _courtDetailBusiness;

        public IndexModel()
        {
            _courtDetailBusiness ??= new CourtDetailBusiness();
        }

        public IList<CourtDetail> CourtDetail { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_courtDetailBusiness != null)
            {
                var result = await _courtDetailBusiness.GetAllCourtDetailsIncludeCourt();
                if (result != null && result.Status > 0 && result.Data != null)
                {
                    CourtDetail = result.Data as List<CourtDetail>;
                }
            }
        }
    }
}
