using System;
using System.Web;

namespace MeatGrinder.Services
{
    public static class CookieService
    {
        public static string GetCookie(HttpContext context, string cookieName)
        {
            var httpCookie = HttpContext.Current.Request.Cookies["meatgrinder"];
            if (httpCookie != null) 
                return httpCookie[cookieName];

            return null;
        }
        public static void SetCookie(HttpResponseBase response, string cookieName, int cookieDuration, string cookieValue = "0")
        {
            var cookie = new HttpCookie("MeatGrinder");

            cookie[cookieName] = cookieValue;
            cookie.Expires = DateTime.Now.AddDays(cookieDuration);
            response.Cookies.Add(cookie);
        }
        public static void DeleteCookie(HttpResponseBase response, string cookieName)
        {
            SetCookie(response, cookieName, -1);
        }
        public static int GetUserID()
        {
            int userID = int.Parse(CookieService.GetCookie(System.Web.HttpContext.Current, "UserID"));
            return userID;
        }

    }
}