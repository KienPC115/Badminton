using Badminton.Business;
using Badminton.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Badminton.RazorWebApp.Pages
{
    public class CourtModel : PageModel
    {
        private readonly ICourtBusiness _courtBusiness = new CourtBusiness();

        public string Message { get; set; } = default;

        public Court Court { get; set; } = default;

        public List<Court> Courts { get; set; } = new List<Court>();

        public void OnGet()
        {
            Courts = this.GetCourts();
        }

        public void OnPost() {
            this.SaveCourt();
        }

        public void OnDelete() {

        }

        private List<Court> GetCourts() {
            var courtsResult = _courtBusiness.GetAllCourts();

            if(courtsResult.Status > 0 && courtsResult.Result.Data != null) {
                var courts = (List<Court>)courtsResult.Result.Data;
                return courts;
            }

            return new List<Court> ();
        }

        private void SaveCourt() {
            var result = _courtBusiness.AddCourt(this.Court);

            if(result != null) {
                this.Message = result.Result.Message;
            }
            else {
                this.Message = "Error System";
            }

        }
    }
}
