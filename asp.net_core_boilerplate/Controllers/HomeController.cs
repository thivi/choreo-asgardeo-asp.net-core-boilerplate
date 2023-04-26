using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using asp.net_core_boilerplte.Utils;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using System.Diagnostics.Metrics;
using asp.net_core_boilerplate.Models;

namespace asp.net_core_boilerplte.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IConfiguration _configuration;

    public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [Authorize]
    public async Task<IActionResult> Secure()
    {
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        var idToken = await HttpContext.GetTokenAsync("id_token");
        var refreshToken = await HttpContext.GetTokenAsync("refresh_token");
        string displayName = UserUtils.GetDisplayName(User);
        IEnumerable<System.Security.Claims.Claim> claims = User.Claims;

        // Getting the profile picture URL from the userinfo endpoint
        // to demonstrate how an API request can be dispatched to a
        // protected endpoint using the access token.
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        string userinfoEndpoint = $"https://api.asgardeo.io/t/{_configuration["Asgardeo:Tenant"]}/oauth2/userinfo";
        using var response = await httpClient.GetAsync(userinfoEndpoint);

        string profilePic = "https://img.freepik.com/free-psd/3d-illustration-person-with-sunglasses_23-2149436188.jpg";

        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();
            dynamic json = JsonConvert.DeserializeObject(content)!;

            profilePic = json.profile;
        }
        
        return View(new Secure {
            Claims = claims,
            AccessToken = accessToken,
            DisplayName = displayName,
            IdToken = idToken,
            RefreshToken = refreshToken,
            ProfileURL = profilePic
        });
    }

    [Authorize]
    public async Task<IActionResult> Choreo()
    {
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        var apiEndpoint = _configuration["Choreo:ApiEndpoint"];

        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await httpClient.GetAsync($"{apiEndpoint}/pet/1");

        string category = "";
        string name = "";
        
        if(response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();
            dynamic json = JsonConvert.DeserializeObject(content)!;

            category = json.category.name;
            name = json.name;
        }

        return View(new Pet
        {
            Category = category,
            Name = name,
            ID = 1
        });
        ;
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

