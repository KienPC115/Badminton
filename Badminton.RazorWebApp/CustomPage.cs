using Badminton.Common;
using Badminton.Data.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Badminton.RazorWebApp {
    public abstract class CustomPage : PageModel{
        public bool IsAdmin { get; set; } = false;

        public bool CheckAdmin() {
            Helpers.GetValueFromSession("cus", out Customer cus, HttpContext);

            if(cus == null) {
                return false;
            }

            if(cus.Email == "admin@example.com" && cus.Name == "admin") {
                return true;
            }

            return false;
        }
    }
}
