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
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Org.BouncyCastle.Crypto.Engines;

namespace Badminton.RazorWebApp.Pages.CourtDetailPage
{
    public class IndexModel : CustomPage
    {
        private readonly ICourtDetailBusiness _courtDetailBusiness;

        public IndexModel(IConfiguration configuration)
        {
            _config = configuration;
            _courtDetailBusiness ??= new CourtDetailBusiness();
        }

        [BindProperty(SupportsGet = true)] public IList<CourtDetail> CourtDetailList { get; set; } = default!;
        public CourtDetail CourtDetail { get; set; } = default!;
        private readonly IConfiguration _config;
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? Slot { get; set; }
        public double? MinPrice { get; set; }
        public double? MaxPrice { get; set; }
        public int? Capacity { get; set; }
        public List<string> SlotList = default!;

        public async Task<IActionResult> OnGetAsync(int newCurPage = 1, string? minPrice = @"",
            string? maxPrice = @"", string? capacity = @"")
        {
            IsAdmin = CheckAdmin();
            SlotList = CourtDetailShared.Slot();
            int pageSize = Int32.TryParse(_config["PageSize"], out int ps) ? ps : 3;
            CurrentPage = newCurPage;
            if (!(string.IsNullOrEmpty(minPrice) && !Double.TryParse(minPrice, out double minValue)) ||
                !(string.IsNullOrEmpty(maxPrice) && !Double.TryParse(maxPrice, out double maxValue)) ||
                !(string.IsNullOrEmpty(capacity) && !Int32.TryParse(capacity, out int capacityValue)))
            {
                TempData["message"] = "min price & max price & capacity must be a number";
                return Page();
            }

            if (!string.IsNullOrEmpty(minPrice))
            {
                MinPrice = minValue;
            }
            
            if (!string.IsNullOrEmpty(minPrice))
            {
                MaxPrice = maxValue;
            }

            if (!string.IsNullOrEmpty(capacity))
            {
                Capacity = capacityValue;
            }
            if (_courtDetailBusiness != null)
            {
                var courtDetail = await _courtDetailBusiness.GetTopBookedCourt();
                var result =
                    await _courtDetailBusiness.GetAllCourtDetailsIncludeCourt(Slot, Capacity, MinPrice, MaxPrice);
                if (result != null && result.Status > 0 && result.Data != null)
                {
                    var list = result.Data as List<CourtDetail>;
                    TotalPages = (int)Math.Ceiling(list.Count / (double)pageSize);
                    CourtDetailList = list.Skip((CurrentPage - 1) * pageSize).Take(pageSize).ToList();
                }

                if (courtDetail != null && courtDetail.Status > 0 && courtDetail.Data != null)
                {
                    CourtDetail = courtDetail.Data as CourtDetail;
                }
            }

            return Page();
        }
    }
}