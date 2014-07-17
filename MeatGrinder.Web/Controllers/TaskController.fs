namespace MeatGrinder.Web.Controllers

open System.Collections.Generic
open System.Linq
open System.Web.Mvc

open MeatGrinder.DAL.Models

open MeatGrinder.Schema

open MeatGrinder.Web.Repositories
open MeatGrinder.Web.Helpers
open MeatGrinder.Web.Services

[<CustomAuthorize>]
type TaskController() =
    inherit Controller()
    let createBreadCrumb description id breadCrumbType =
        {DisplayName=description;Url=sprintf "#%s/%i" breadCrumbType id}
    let createBreadCrumbFromGoal (goal:Goal) =
        createBreadCrumb goal.Description goal.ID "goal"
    
    let createBreadCrumbsFromTask (tRepo:TaskRepository) (gRepo:GoalRepository) (task:Task) =
        let taskSeq=
            task 
            |> Seq.unfold (function
                    |t when t.ParentTaskID.HasValue -> Some(t,tRepo.GetById(t.ParentTaskID.Value))
                    | _ -> None)
            |> Seq.toArray
        let crumbs = taskSeq        |> Array.map (fun t->createBreadCrumb task.Description task.ID "task" ) |> Array.toList
        let goal = taskSeq.Last().GoalID |> gRepo.GetById
        if goal <> null then 
             createBreadCrumbFromGoal(goal)::crumbs
        else crumbs
        
    [<HttpPost>]
    member x.GetTasksForGoal(goalId) =
        let viewModel = TaskViewModel()
        use db= new MeatGrinderEntities()
        let goal = GoalRepository(db,CookieService.GetUserId).GetById goalId
        if goal<>null then
            viewModel.Tasks <- TaskRepository(db,CookieService.GetUserId).GetAllByGoalId(goalId).ToList()
            viewModel.BreadCrumbs <- [createBreadCrumbFromGoal(goal)].ToList()
        x.Json(viewModel)
    [<HttpPost>]
    member x.GetTasksForTask(taskId) =
        let viewModel = TaskViewModel()
        use db = new MeatGrinderEntities()
        let tRepo = TaskRepository(db,CookieService.GetUserId)
        let gRepo = GoalRepository(db,CookieService.GetUserId)
        let task = tRepo.GetById taskId
        if task<>null then
            viewModel.Tasks <- tRepo.GetAllByTaskId(taskId).ToList()
            viewModel.BreadCrumbs <- (createBreadCrumbsFromTask tRepo gRepo task).ToList()
        tRepo.UpdateChildTaskCounts(viewModel.Tasks |> Seq.toList)
        x.Json(viewModel)
    member x.Create (task:Task) =
        if x.ModelState.IsValid then 
            use db = new MeatGrinderEntities()
            let tRepo = TaskRepository(db,CookieService.GetUserId)
            let gRepo = GoalRepository(db,CookieService.GetUserId)
            tRepo.Create task gRepo.SetGoalNotComplete
    member x.Delete taskId =
        use db = new MeatGrinderEntities()
        let tRepo = TaskRepository(db,CookieService.GetUserId)
        tRepo.Delete taskId
