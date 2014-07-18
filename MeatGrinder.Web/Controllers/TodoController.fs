﻿namespace MeatGrinder.Controllers

open System.Collections.Generic
open System.Data.Objects
open System.Linq
open System.Web.Mvc

open Microsoft.FSharp.Linq.NullableOperators

open MeatGrinder.Schema

open MeatGrinder.DAL.Models
open MeatGrinder.Web.Repositories
open MeatGrinder.Web.Helpers
open MeatGrinder.Web.Services

[<CustomAuthorize>]
type TodoController() =
    inherit Controller()

    //use MeatGrinderEntities _db = new MeatGrinderEntities()
    member private x.AddTaskWithNoTasksToList (todoList:List<TodoViewModel>) (parentTask:Task) (db:MeatGrinderEntities) =
        let goal = db.Goals.FirstOrDefault( fun g -> g.ID = parentTask.GoalID)
        let tempTask = db.Tasks.FirstOrDefault(fun t -> t.ID =? parentTask.ParentTaskID)
        let todo = {Id=parentTask.ID
                    Description=parentTask.Description
                    TaskType= "Task"
                    GoalName=if goal<>null then goal.Description else null
                    ParentTaskName = if tempTask<>null then tempTask.Description else null
                    }
        todoList.Add(todo);

    member private x.GetTodoListTasks(tasks:IEnumerable<Task>, userId,todoList:List<TodoViewModel>,db:MeatGrinderEntities) =
        let rec getTodoListTasks (childTasks:IEnumerable<Task>) =
            childTasks
            |> Seq.iter (fun parentTask -> 
                let childTasks = db.Tasks.Where(fun m-> m.IsComplete = false && m.ParentTaskID ?= parentTask.ID && m.UserID = userId)
                if(childTasks.Any()=false)
                    then x.AddTaskWithNoTasksToList todoList parentTask db
                    else getTodoListTasks(childTasks)
                )
        getTodoListTasks tasks
   
    member private x.GetToDoList() = 
        let todoList = new List<TodoViewModel>();
        let userId = CookieService.GetUserId().Value
        use db= new MeatGrinderEntities()
        db.Goals.Where(fun m->m.IsComplete=false && m.UserID=userId)
        |> Seq.iter (fun g-> 
            let tasks = db.Tasks.Where(fun t->t.IsComplete=false && t.GoalID = g.ID && t.ParentTaskID.HasValue=false && t.UserID = userId)
            if tasks.Any() 
            then todoList.Add({Id=g.ID;Description= g.Description;TaskType= "Goal";ParentTaskName=null;GoalName=null})
            else x.GetTodoListTasks(tasks,userId,todoList,db )
            )
        todoList

    member x.GetList() =
        let viewModel = x.GetToDoList()
        x.Json(viewModel, JsonRequestBehavior.AllowGet)
         
    [<HttpPost>]
    member x.Complete(viewModel:TodoViewModel) =
        use db = new MeatGrinderEntities()
        match viewModel.TaskType with
        |"Task" -> 
            let task = db.Tasks.FirstOrDefault(fun m->m.ID=viewModel.Id)
            if task <> null then
                task.IsComplete <- true
                db.SaveChanges() |> ignore
        |"Goal" ->
            let goal = db.Goals.FirstOrDefault(fun m->m.ID=viewModel.Id)
            if goal<> null then
                goal.IsComplete <- true
                db.SaveChanges() |> ignore
        ()