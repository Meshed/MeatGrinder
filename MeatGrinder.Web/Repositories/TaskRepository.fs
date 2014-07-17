namespace MeatGrinder.Web.Repositories
open System.Linq
open Microsoft.FSharp.Linq.NullableOperators

open MeatGrinder.DAL.Models

type TaskRepository(db:MeatGrinderEntities, getUserId:unit->int option) =
    member x.GetById id = db.Tasks.FirstOrDefault(fun m-> m.ID=id)
    member x.GetAllByGoalId goalId = 
        let id = getUserId().Value
        db.Tasks.Where(fun m->m.GoalID=goalId && m.ParentTaskID.HasValue=false && m.UserID= id) |> Seq.toList
    member x.GetAllByTaskId (taskId:int) =
        let userId = getUserId().Value
        db.Tasks.Where(fun m->m.ParentTaskID ?= taskId && m.UserID=userId) |> Seq.toList
    member x.GetChildTaskCount taskId =
        db.Tasks.Count (fun m-> m.ParentTaskID ?= taskId)
    member x.SetTaskNotComplete taskId =
        let task = db.Tasks.FirstOrDefault(fun m->m.ID=taskId)
        if(task<>null && task.IsComplete) then
            task.IsComplete <- false
            db.SaveChanges() |> ignore
    member x.Create (task:Task) setGoalNotComplete =
        task.UserID <- getUserId().Value
        task.IsComplete <- false
        db.Tasks.Add task |> ignore
        db.SaveChanges() |> ignore
        if task.ParentTaskID.HasValue
        then x.SetTaskNotComplete task.ParentTaskID.Value
        else setGoalNotComplete task.GoalID
    
    member x.DeleteTask (task:Task) =
        let rec delete (parentTask:Task) =
            db.Tasks.Where(fun m->m.ParentTaskID ?= parentTask.ID) 
                |> Seq.toList
                |> Seq.iter (fun t-> x.DeleteTask t)
        delete task
        db.Tasks.Remove(task) |> ignore
        
    member x.Delete taskId =
        let task = db.Tasks.FirstOrDefault(fun m->m.ID=taskId)

        if task<>null then
            x.DeleteTask task
            db.SaveChanges() |> ignore
    member x.UpdateChildTaskCounts (tasks:Task list) =
        let upd (t:Task) = 
            t.ChildTaskCount <- x.GetChildTaskCount(t.ID)
            if t.ChildTaskCount>0 then t.Description <- sprintf "%s(%i)" t.Description t.ChildTaskCount
        tasks |> Seq.iter upd
