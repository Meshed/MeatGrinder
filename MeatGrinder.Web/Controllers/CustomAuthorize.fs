module MeatGrinder.Web.Helpers
open System
open MeatGrinder.Web.Services
type CustomAuthorize () =
    inherit System.Web.Mvc.AuthorizeAttribute()
    override x.AuthorizeCore(httpContext) =
        let userId = 0
        let cookieValue = CookieService.GetCookie httpContext "UserID"
        if cookieValue.IsSome && cookieValue.Value<>null then Int32.Parse(cookieValue.Value)<>0 else false


