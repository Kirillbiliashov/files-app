using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using FilesApp.Models.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FilesApp.Controllers.API
{
    public class BaseApiController : ControllerBase
    {
        protected string? UserId => 
        User.Identity.IsAuthenticated ? JsonSerializer.Deserialize<CookieUser>(User.FindFirst("CookieUser")?.Value)?.Id : null;
    }
}