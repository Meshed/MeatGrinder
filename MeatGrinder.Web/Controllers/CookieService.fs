namespace MeatGrinder.Web.Services
open System
open System.Web
[<Sealed>]
[<AbstractClass>]
type CookieService =
    static member GetCookie (context:HttpContextBase) cookieName =
        let httpCookie = context.Request.Cookies.["MeatGrinder"]
        if httpCookie<>null then Some(httpCookie.[cookieName]) else None
    static member SetCookie (response:HttpResponseBase, cookieName, cookieDuration:int, cookieValue) =
        let cookie = new HttpCookie("MeatGrinder")
        cookie.[cookieName] <- cookieValue
        cookie.Expires <- DateTime.Now.AddDays (float(cookieDuration))
        response.Cookies.Add(cookie)
    static member SetCookie (response:HttpResponseBase, cookieName, cookieDuration) =
        CookieService.SetCookie(response, cookieName, cookieDuration, "0")
    static member DeleteCookie response cookieName =
        CookieService.SetCookie(response, cookieName, -1)
    static member GetUserId () =
        let context = System.Web.HttpContextWrapper System.Web.HttpContext.Current
        let cookie = CookieService.GetCookie context "UserId"
        if cookie.IsSome && cookie.Value<> null then Some(Int32.Parse(cookie.Value)) else None
