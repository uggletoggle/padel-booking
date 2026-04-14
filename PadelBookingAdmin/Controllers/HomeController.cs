using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PadelBookingAdmin.Models;
using System.Diagnostics;

namespace PadelBookingAdmin.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Profile()
        {
            return View(User.Claims.ToList());
        }

        public IActionResult Login(string returnUrl = "/")
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return Redirect(returnUrl);
            }
            return Challenge(new AuthenticationProperties { RedirectUri = returnUrl }, OpenIdConnectDefaults.AuthenticationScheme);
        }

        public IActionResult Logout()
        {
            return SignOut(new AuthenticationProperties { RedirectUri = "/" }, 
                CookieAuthenticationDefaults.AuthenticationScheme, 
                OpenIdConnectDefaults.AuthenticationScheme);
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            
            return View(new ErrorViewModel 
            { 
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                ExceptionMessage = exceptionHandlerPathFeature?.Error?.Message,
                StackTrace = exceptionHandlerPathFeature?.Error?.StackTrace
            });
        }
    }
}
