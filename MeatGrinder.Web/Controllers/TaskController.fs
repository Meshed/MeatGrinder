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
        
        use db= new MeatGrinderEntities()
        let goal = GoalRepository(db,CookieService.GetUserId).GetById goalId
        if goal<>null then
            let breadCrumbs = [createBreadCrumbFromGoal(goal)].ToList()
            let tasks = TaskRepository(db,CookieService.GetUserId).GetAllByGoalId(goalId).ToList()
            let viewModel = TaskViewModel(breadCrumbs,tasks)
            x.Json(viewModel)
        else
            let viewModel = TaskViewModel(null,null)
            x.Json(viewModel)
    [<HttpPost>]
    member x.GetTasksForTask(taskId) =
        
        use db = new MeatGrinderEntities()
        let tRepo = TaskRepository(db,CookieService.GetUserId)
        let gRepo = GoalRepository(db,CookieService.GetUserId)
        let task = tRepo.GetById taskId
        if task<>null then

            let tasks = tRepo.GetAllByTaskId(taskId).ToList()
            let breadCrumbs = (createBreadCrumbsFromTask tRepo gRepo task).ToList()
            tRepo.UpdateChildTaskCounts(tasks |> Seq.toList)
            let viewModel = new TaskViewModel(breadCrumbs,tasks)
            x.Json(viewModel)
        else
            let viewModel = TaskViewModel(null,null)
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
