using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace asp.net_core_boilerplte.Controllers
{
    public class IdentityController : Controller
    {
        private readonly IConfiguration _configuration;

        public IdentityController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            var redirectUrl = Url.Action("Index", "Home", null, Request.Scheme);

            return Challenge(
                new AuthenticationProperties { RedirectUri = redirectUrl },
                OpenIdConnectDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var idToken = await HttpContext.GetTokenAsync("id_token");
            string appOrigin = Url.Action("Index", "Home", null, Request.Scheme)!;
            string redirectURL = $"{appOrigin.Remove(appOrigin.Length - 1, 1)}";

            return Redirect($"https://api.asgardeo.io/t/{_configuration["Asgardeo:Tenant"]}/oidc/logout?id_token_hint={idToken}&post_logout_redirect_uri={redirectURL}");
        }
    }
}

