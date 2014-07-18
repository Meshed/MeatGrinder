namespace MeatGrinder.Web.Controllers

open System.Collections.Generic
open System.Linq
open System.Web.Mvc

open MeatGrinder.Orm
open MeatGrinder.Schema
open MeatGrinder.Web.Repositories
open MeatGrinder.Web.Helpers
open MeatGrinder.Web.Services

[<CustomAuthorize>]
type AdminController() =
    inherit Controller()
    let allowGet= JsonRequestBehavior.AllowGet
    member x.Summary =
        use db= new MeatGrinderEntities()
        let viewModel = {TotalUserCount=db.Users.Count()}
        x.Json(viewModel, allowGet)
    [<HttpGet>]
    member x.IsUserAdmin () =
        use db = new MeatGrinderEntities()
        let isUserAdmin = 
            let userId = CookieService.GetUserId()
            if userId.IsSome then
                let user = db.Users.FirstOrDefault(fun m->m.Id=userId.Value)
                match user with
                | admin when admin.EmailAddress="marksbrown@gmail.com" -> true
                | _ -> false
            else false
        x.Json(isUserAdmin, allowGet)
    
