using Badminton.Business;
using Badminton.Business.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Badminton.RazorWebApp.Pages.Court
{
    public class CourtModel : PageModel
    {
        private readonly ICourtBusiness _courtBusiness;
        public IBadmintonResult result;
        public IList<int> Courts { get; set; }

        public CourtModel()
        {
            _courtBusiness ??= new CourtBusiness();
        }

        public async void OnGet()
        {
            result = await _courtBusiness.GetAllCourts();
            Courts = new List<int>() { 1, 2, 3, 4, 5};
        }
    }
}
