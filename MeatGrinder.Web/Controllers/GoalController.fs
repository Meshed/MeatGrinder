﻿namespace MeatGrinder.Web.Controllers
open System.Collections.Generic
open System.Linq
open System.Web.Mvc

open MeatGrinder.DAL.Models
open MeatGrinder.Web.Repositories
open MeatGrinder.Web.Helpers
open MeatGrinder.Web.Services

//open MeatGrinder.Repositories
[<CustomAuthorize>]
type GoalController () =
    inherit Controller ()
    let goalRepository = new GoalRepository(new MeatGrinderEntities(), CookieService.GetUserId)
    
    member x.GetAll() = 
        let goals =  goalRepository.GetAllForUser().ToList()
        let viewModel = GoalViewModel(Goals=goals)
        goalRepository.UpdateChildTaskCounts <| (viewModel.Goals |> Seq.toList)
        x.Json(viewModel,JsonRequestBehavior.AllowGet)
    member x.Create goal =
        if x.ModelState.IsValid then goalRepository.Create goal |> ignore
    member x.Delete goal = goalRepository.Delete goal
        
