using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using FilesApp.Attributes;
using FilesApp.DAL;
using FilesApp.Models.Auth;
using FilesApp.Models.DAL;
using FilesApp.Models.Http;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FilesApp.Controllers.API
{
    [ApiController]
    [Route("api/auth")]
    public class AuthApiController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _config;

        public AuthApiController(UserManager<AppUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }


        [HttpPost("register")]
        public async Task<IActionResult> RegisterNewUser([FromBody] RegisterUserBody body)
        {
            var user = new AppUser
            {
                UserName = body.Email.Split("@").FirstOrDefault(),
                FirstName = body.FirstName,
                LastName = body.LastName,
                Email = body.Email
            };

            var result = await _userManager.CreateAsync(user, body.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
            }

            var cookieUser = await SetCookies(user);

            return Ok(cookieUser);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginUserBody body)
        {
            Console.WriteLine($"inside login, body: {JsonSerializer.Serialize(body)}");
            var user = await _userManager.FindByEmailAsync(body.Email);
            if (user == null)
            {
                return NotFound();
            }

            var isValidPassword = await _userManager.CheckPasswordAsync(user, body.Password);
            if (!isValidPassword)
            {
                return Unauthorized();
            }

            var cookieUser = await SetCookies(user);

            return Ok(cookieUser);
        }

        [HttpGet("login/google")]
        public async Task<IActionResult> LoginWithGoogle()
        {
            var returnUrl = new Uri("https://localhost:44485/auth/google/callback");
            var defaultScopes = new[] { "https://www.googleapis.com/auth/userinfo.profile", "https://www.googleapis.com/auth/userinfo.email" };
            var uriBuilder = "https://accounts.google.com/o/oauth2/v2/auth";
            uriBuilder += "?client_id=" + _config["Authentication:Google:ClientId"];
            uriBuilder += "&redirect_uri=" + returnUrl.GetLeftPart(UriPartial.Path);
            uriBuilder += "&response_type=code";
            uriBuilder += "&access_type=offline";
            uriBuilder += "&include_granted_scopes=true";
            uriBuilder += "&scope=" + string.Join(" ", defaultScopes);
            uriBuilder += "&state=state";
            Console.WriteLine($"uri builder: {uriBuilder}");
            return Redirect(uriBuilder);
        }

        [HttpPost("login/google/process")]
        public async Task<IActionResult> ProcessGoogleLogin([FromBody] ProcessGoogleLoginBody body)
        {
            var accessCode = await GetAccessCode(body.code);
            Console.WriteLine($"access code: {accessCode}");
            var payload = await GoogleJsonWebSignature.ValidateAsync(accessCode);

            if (payload != null)
            {
                Console.WriteLine($"Email: {payload.Email}");
                var user = await _userManager.FindByEmailAsync(payload.Email);
                if (user == null)
                {
                    user = new AppUser
                    {
                        UserName = payload.Email.Split("@").FirstOrDefault(),
                        FirstName = payload.GivenName,
                        LastName = payload.FamilyName,
                        Email = payload.Email
                    };
                    await _userManager.CreateAsync(user);
                }

                Console.WriteLine($"found user, setting cookies");

                var cookieUser = await SetCookies(user);
                return Ok(cookieUser);
            }

            return Unauthorized();
        }

        private async Task<string?> GetAccessCode(string authCode)
        {
            var endpoint = "https://oauth2.googleapis.com/token";
            var payload = new Dictionary<string, string>
            {
                { "code", authCode },
                { "client_id", _config["Authentication:Google:ClientId"] },
                { "client_secret", _config["Authentication:Google:ClientSecret"] },
                { "redirect_uri", "https://localhost:44485/auth/google/callback" },
                { "grant_type", "authorization_code" }
            };

            using var httpClient = new HttpClient();
            var tokenResponse = await httpClient.PostAsync(endpoint, new FormUrlEncodedContent(payload));
            var tokenResponseBody = await tokenResponse.Content.ReadAsStringAsync();
            Console.WriteLine($"token response: {tokenResponseBody}");
            if (tokenResponse.IsSuccessStatusCode)
            {
                var tokenResult = JsonSerializer.Deserialize<GoogleTokenResponse>(tokenResponseBody);
                return tokenResult?.IdToken;
            }

            return null;
        }

        private async Task<CookieUser> SetCookies(AppUser user)
        {
            var cookieUser = new CookieUser
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email
            };
            var cookieUserStr = JsonSerializer.Serialize(cookieUser);

            var claims = new List<Claim>
            {
                new Claim("CookieUser", cookieUserStr)
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = true,
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.AddMonths(6)
            };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            return cookieUser;
        }

        [AllowOnlyAuthorized]
        [HttpGet("logout")]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }
    }
}