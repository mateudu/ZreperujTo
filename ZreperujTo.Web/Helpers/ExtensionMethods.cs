using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ZreperujTo.Web.Helpers
{
    public static class ExtensionMethods
    {
        public static string GetSubId(this System.Security.Claims.ClaimsPrincipal user)
        {
            return user.Claims.FirstOrDefault(
                        x => x.Type == @"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
        }
    }
}
