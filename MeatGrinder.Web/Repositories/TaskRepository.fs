namespace MeatGrinder.Web.Repositories
open System.Linq
open Microsoft.FSharp.Linq.NullableOperators

open MeatGrinder.Orm

type TaskRepository(db:MeatGrinderEntities, getUserId:unit->int option) =
    member x.GetById id = db.Tasks.FirstOrDefault(fun m-> m.Id=id)
    member x.GetAllByGoalId goalId = 
        let id = getUserId().Value
        db.Tasks.Where(fun m->m.GoalId=goalId && m.ParentTaskID.HasValue=false && m.UserId= id) |> Seq.toList
    member x.GetAllByTaskId (taskId:int) =
        let userId = getUserId().Value
        db.Tasks.Where(fun m->m.ParentTaskID ?= taskId && m.UserId=userId) |> Seq.toList
    member x.GetChildTaskCount taskId =
        db.Tasks.Count (fun m-> m.ParentTaskID ?= taskId)
    member x.SetTaskNotComplete taskId =
        let task = db.Tasks.FirstOrDefault(fun m->m.Id=taskId)
        if task.IsComplete then
            task.IsComplete <- false
            db.SaveChanges() |> ignore
    member x.Create (task:Task) setGoalNotComplete =
        task.UserId <- getUserId().Value
        task.IsComplete <- false
        db.Tasks.Add task |> ignore
        db.SaveChanges() |> ignore
        if task.ParentTaskID.HasValue
        then x.SetTaskNotComplete task.ParentTaskID.Value
        else setGoalNotComplete task.GoalId
    
    member x.DeleteTask (task:Task) =
        let rec delete (parentTask:Task) =
            db.Tasks.Where(fun m->m.ParentTaskID ?= parentTask.Id) 
                |> Seq.toList
                |> Seq.iter (fun t-> x.DeleteTask t)
        delete task
        db.Tasks.Remove(task) |> ignore
        
    member x.Delete taskId =
        let task = db.Tasks.FirstOrDefault(fun m->m.Id=taskId)

        if task.Id>0 then
            x.DeleteTask task
            db.SaveChanges() |> ignore
    member x.UpdateChildTaskCounts (tasks:Task list) =
        let upd (t:Task) = 
            let childTaskCount = x.GetChildTaskCount(t.Id)
            t.ChildTaskCount <- Some(childTaskCount)
            if t.ChildTaskCount.Value>0 then t.Description <- sprintf "%s(%i)" t.Description childTaskCount
        tasks |> Seq.iter upd
