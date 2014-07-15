module MeatGrinder.Web.Services
open System.Web
[<Sealed>]
[<AbstractClass>]
type CookieService =
    static member GetCookie (context:HttpContextBase) cookieName =
        let httpCookie = context.Request.Cookies.["meatgrinder"]
        if httpCookie<>null then Some(httpCookie.[cookieName]) else None
    static member SetCookie (response:HttpResponseBase, cookieName, cookieDuration, cookieValue) =
        ()
    static member SetCookie (response:HttpResponseBase, cookieName, cookieDuration) =
        CookieService.SetCookie(response, cookieName, cookieDuration, "0")

