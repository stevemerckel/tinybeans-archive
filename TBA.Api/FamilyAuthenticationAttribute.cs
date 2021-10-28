using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TBA.Api
{
    /// <summary>
    /// Checks authorization for this request
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public class FamilyAuthenticationAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            const string CookieName = "mySpecialKey";
            Debug.WriteLine($"Running {nameof(OnAuthorization)}");
            context.HttpContext.Request.Cookies.TryGetValue(CookieName, out var cookieValue);
            var isAllowed = Debugger.IsAttached || IsCookieValid(cookieValue);

            if (isAllowed)
                return;

            context.Result = new UnauthorizedResult();
        }

        private static bool IsCookieValid(string cookieValue)
        {
            return true; // todo: implement against actual backing service
        }
    }
}