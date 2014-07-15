namespace MeatGrinder.Web.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Web
open System.Web.Mvc
open System.Web.Mvc.Ajax
open MeatGrinder.Web.Helpers
#if DEBUG
#else
[<CustomAuthorize>]
#endif

type HomeController() =
    inherit Controller()
    member this.Index () = this.View()

