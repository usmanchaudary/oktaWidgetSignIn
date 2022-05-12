using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Okta.AspNetCore;
using RestSharp;
using System;
using System.Threading.Tasks;
using System.Web;

namespace okta_aspnetcore_mvc_example.Controllers
{
    [Route("authorization-code")]
    public class AccountController : Controller
    {
        [HttpGet("signin")]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpGet("callback")]
        public async Task<IActionResult> SignIn([FromForm]string sessionToken)
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                var properties = new AuthenticationProperties();
                properties.Items.Add("sessionToken", sessionToken);
                properties.RedirectUri = "/Home/";

                var client = new RestClient("https://dev-12063452.okta.com:443/oauth2/default/v1/token");
                var code = HttpContext.Request.Query["code"].ToString();

                var request = new RestRequest();
                request.Method = Method.Post;
                request.AddHeader("Accept", "*/*");
                request.AddHeader("Accept-Encoding", "gzip, deflate");
                request.AddHeader("Authorization", "Basic MG9hNHl6YmFqNzNuN1I3dFY1ZDc6ZzZmTDVXbk9hRGx1WlJXNkZUcUtTZVBOdkdFb0JqNTdBUmcweUlGVg==");
                request.AddHeader("Connection", "keep-alive");
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
               // request.AddHeader("Cookie", "JSESSIONID=993124FAAD9CCBDE95302C3CDDDD5225");
                request.AddParameter("grant_type", "authorization_code");
                request.AddParameter("code", code);
                request.AddParameter("redirect_uri", "https://localhost:44314/authorization-code/callback");
                RestResponse response =await client.ExecuteAsync(request);
                Console.WriteLine(response.Content);

                return Challenge(properties, OktaDefaults.MvcAuthenticationScheme);//.ExecuteResult(ac);
                 
                 //Ok("done");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult SignOut()
        {
            return new SignOutResult(
                new[]
                {
                     OktaDefaults.MvcAuthenticationScheme,
                     CookieAuthenticationDefaults.AuthenticationScheme,
                },
                new AuthenticationProperties { RedirectUri = "/Home/" });
        }
    }
}