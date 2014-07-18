namespace MeatGrinder.Web.Repositories
open System.Linq
//open Microsoft.FSharp.Linq.NullableOperators

open MeatGrinder.Orm

type GoalRepository (db:MeatGrinderEntities, getUserId:unit-> int option)=
    member x.GetById id =
        db.Goals.FirstOrDefault(fun m-> m.Id=id)
    member x.GetChildTaskCount goalId =
        db.Tasks.Count(fun m-> m.GoalId = goalId && m.ParentTaskID.HasValue=false)
    member x.SetGoalNotComplete goalId =
        let goal = db.Goals.FirstOrDefault(fun m->m.Id=goalId)
        if goal.Id>0 && goal.IsComplete then
            goal.IsComplete <- false
            db.SaveChanges() |> ignore
    member x.GetAllForUser () =
        let userId = getUserId().Value
        db.Goals.Where(fun m->m.UserId = userId) |> Seq.toList
    member x.Create (goal:Goal) =
        goal.UserId <- getUserId().Value
        goal.IsComplete <- false
        db.SaveChanges()
    member x.Delete deleteTasks goalId =
        let goal = db.Goals.FirstOrDefault(fun m->m.Id=goalId)
        if goal.Id>0 then
            deleteTasks <|| (db,db.Tasks.Where(fun m->m.GoalId = goalId))
            db.Goals.Remove goal |> ignore
            db.SaveChanges() |> ignore
    member x.UpdateChildTaskCounts (goals:Goal list) =
        let upd (g:Goal) = 
            g.ChildTaskCount <- Some(x.GetChildTaskCount(g.Id))
            if g.ChildTaskCount.Value > 0 then g.Description <- sprintf "%s(%i)" g.Description g.ChildTaskCount.Value
        goals |> Seq.iter upd
