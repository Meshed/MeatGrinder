namespace MeatGrinder.Web.Repositories
open System.Linq

open MeatGrinder.DAL.Models

type GoalRepository (db:MeatGrinderEntities, getUserId:unit-> int option)=
    member x.GetById id =
        db.Goals.FirstOrDefault(fun m-> m.ID=id)
    member x.GetChildTaskCount goalId =
        db.Tasks.Count(fun m-> m.GoalID = goalId && m.ParentTaskID.HasValue=false)
    member x.SetGoalNotComplete goalId =
        let goal = db.Goals.FirstOrDefault(fun m->m.ID=goalId)
        if goal<>null && goal.IsComplete then
            goal.IsComplete <- false
            db.SaveChanges() |> ignore
    member x.GetAllForUser () =
        let userId = getUserId().Value
        db.Goals.Where(fun m->m.UserID = userId) |> Seq.toList
    member x.Create (goal:Goal) =
        goal.UserID <- getUserId().Value
        goal.IsComplete <- false
        db.SaveChanges()
    member x.Delete deleteTasks goalId =
        let goal = db.Goals.FirstOrDefault(fun m->m.ID=goalId)
        if goal<> null then
            deleteTasks <|| (db,db.Tasks.Where(fun m->m.GoalID = goalId))
            db.Goals.Remove goal |> ignore
            db.SaveChanges() |> ignore
    member x.UpdateChildTaskCounts (goals:Goal list) =
        let upd (g:Goal) = 
            g.ChildTaskCount <- x.GetChildTaskCount(g.ID)
            if g.ChildTaskCount>0 then g.Description <- sprintf "%s(%i)" g.Description g.ChildTaskCount
        goals |> Seq.iter upd
