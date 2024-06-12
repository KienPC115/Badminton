using Badminton.Business;
using Badminton.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Badminton.RazorWebApp.Pages
{
    public class CourtDetailModel : PageModel
    {

        private readonly ICourtDetailBusiness _courtDetailBusiness = new CourtDetailBusiness();

        public string Message { get; set; } = default;

        [BindProperty]
        public CourtDetail CourtDetail { get; set; } = default;

        public List<CourtDetail> CourtDetails { get; set; } = new List<CourtDetail>();

        public IActionResult OnGet()
        {
            CourtDetails = this.GetCourtDetails();
            return Page();
        }

        public IActionResult OnPost()
        {
            this.SaveCourtDetail();
            CourtDetail = default;
            return Page();
        }

        public IActionResult OnGetGetCourtDetail(int? id)
        {
            if (id != null)
            {
                CourtDetail = (CourtDetail)_courtDetailBusiness.GetCourtDetail(id.Value).Result.Data;
            }
            return RedirectToPage("CourtDetail");
        }
        public IActionResult OnGetDelete(int? id)
        {
            if (id != null)
            {
                var result = _courtDetailBusiness.DeleteCourtDetail(id.Value);
                this.Message = result != null ? result.Result.Message : "Error System";
            }
            return RedirectToPage("CourtDetail");
        }

        private List<CourtDetail> GetCourtDetails()
        {
            var courtDetailsResult = _courtDetailBusiness.GetAllCourtDetailsIncludeCourt();

            if (courtDetailsResult.Status > 0 && courtDetailsResult.Result.Data != null)
            {
                var courtDetails = (List<CourtDetail>)courtDetailsResult.Result.Data;
                return courtDetails;
            }

            return new List<CourtDetail>();
        }

        private void SaveCourtDetail()
        {
            var result = _courtDetailBusiness.AddCourtDetail(this.CourtDetail);

            this.Message = result != null ? result.Result.Message : "Error System";

            CourtDetail = default;
        }
    }

}
