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

        public AuthApiController(UserManager<AppUser> userManager) => _userManager = userManager;
        

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

            await SetCookies(user);

            return Ok(new { });

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

            await SetCookies(user);

            return Ok(new { });
        }

        private async Task SetCookies(AppUser user)
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
        }

        [AllowOnlyAuthorized]
        [HttpGet("logout")]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { });
        }
    }
}