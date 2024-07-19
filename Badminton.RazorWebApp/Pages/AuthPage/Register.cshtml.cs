using Badminton.Business;
using Badminton.Common;
using Badminton.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using static System.Net.WebRequestMethods;

namespace Badminton.RazorWebApp.Pages.AuthPage
{
    public class RegisterModel : PageModel
    {
        private readonly ICustomerBusiness _customerBusiness;
        public RegisterModel()
        {
            _customerBusiness ??= new CustomerBusiness();
        }
        [BindProperty]
        public Customer Customer { get; set; } = new Customer();
        [BindProperty]
        public string OTP { get; set; }
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
                    Customer.Email = email;
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

                var result = await _customerBusiness.AddCustomer(Customer);
                if (result.Status != Const.SUCCESS_CREATE_CODE)
                {
                    TempData["message"] = result.Message;
                    return Page();
                }

                TempData["message"] = "Create account successfully! You can login now.";
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
