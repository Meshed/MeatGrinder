using System.Web;
using System.Web.Mvc;
using MeatGrinder.Services;

namespace MeatGrinder.Helpers
{
    public class CustomAuthorize : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            int userID = 0;

            var cookieValue = CookieService.GetCookie(HttpContext.Current, "UserID");
            if (cookieValue != null)
                userID = int.Parse(cookieValue);

            return userID != 0;
        }
    }
}