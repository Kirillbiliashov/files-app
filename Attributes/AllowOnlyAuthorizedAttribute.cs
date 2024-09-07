using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FilesApp.Attributes
{
    public class AllowOnlyAuthorizedAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var identity = context.HttpContext.User.Identity;
            if (identity == null || !identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}