using Badminton.Business;
using Badminton.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;

namespace Badminton.RazorWebApp.Pages.AuthPage
{
    public class ForgotPasswordPageModel : PageModel
    {
        private readonly ICustomerBusiness _customerBusiness;
        [BindProperty]
        public string OTP { get; set; }
        [BindProperty]
        public string Password { get; set; }
        public ForgotPasswordPageModel()
        {
            _customerBusiness ??= new CustomerBusiness();
        }
        public async Task<IActionResult> OnGet(string? email)
        {
            if (!email.IsNullOrEmpty())
            {
                try
                {
                    var otp = Guid.NewGuid().ToString();
                    var check = await Helpers.SendMail(email, "OTP", otp);

                    if (check.IsNullOrEmpty())
                    {
                        TempData["message"] = "Email is incorrect! Please try with another email.";
                        return Page();
                    }

                    Helpers.SetValueToSession("otp", otp, HttpContext);
                    Helpers.SetValueToSession("email", email, HttpContext);
                    TempData["message"] = "Check your email to get OTP.";
                    return Page();
                }
                catch (Exception)
                {
                    TempData["message"] = "Something went wrong! Please try with another email.";
                    return Page();
                }
            }
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!Helpers.GetValueFromSession("email", out string email, HttpContext) || !Helpers.GetValueFromSession("otp", out string otp, HttpContext))
                {
                    TempData["message"] = "Please try again!";
                    return Page();
                }

                if (!string.Equals(OTP, otp, StringComparison.OrdinalIgnoreCase))
                {
                    TempData["message"] = "Your OTP is not correct! Please check your email again.";
                    return Page();
                }

                var result = await _customerBusiness.ResetPassword(email, Password);
                if (result.Status == Const.WARNING_NO_DATA_CODE)
                {
                    TempData["message"] = "Your email does not exist in the system.";
                    return Page();
                }

                TempData["message"] = "Reset password successfully! You can login now.";
                return RedirectToPage("../Index");
            }
            catch (Exception)
            {
                TempData["message"] = "Something went wrong! Please try with another email.";
                return Page();
            }
        }
    }
}
