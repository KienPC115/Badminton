using Badminton.Business;
using Badminton.Business.Helpers;
using Badminton.Common;
using Badminton.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Text;
using System.Text.Unicode;

namespace Badminton.RazorWebApp.Pages {
    public class IndexModel : PageModel {
        private readonly ICustomerBusiness _login;

        [BindProperty]
        public string Email { get; set; }
        
        [BindProperty]
        public string Password { get; set; }

        public IndexModel()
        {
            _login ??= new CustomerBusiness();
        }

        public IActionResult OnGet()
        {
            HttpContext.Session.Clear();
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            var result = await _login.CheckLogin(Email, Password);
            if (result.Status != Const.SUCCESS_READ_CODE)
            {
                TempData["message"] = "Login failed!!! Please try with another email or password";
                return Page();
            }
            //HttpContext.Session.Set("cus", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(result.Data as Customer)));
            Helpers.SetValueToSession("cus", result.Data as Customer, HttpContext);
            return RedirectToPage("./OrderPage/Index");
        }
    }
}