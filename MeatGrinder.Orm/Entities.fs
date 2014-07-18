namespace MeatGrinder.Orm
open System
open System.Data.Entity
open System.Data.Entity.Infrastructure
open System.Data.Objects
open System.Data.Objects.DataClasses
open System.Linq

open MeatGrinder.Schema

type Goal = {mutable Id:int;mutable Description:string;mutable UserId:int;mutable IsComplete:bool;mutable ChildTaskCount:int option}
type User = {mutable Id:int;EmailAddress:string;AccountName:string;Password:string;DateCreated:Nullable<DateTime>}
type Task = {mutable Id:int;GoalId:int;mutable Description:string;ParentTaskID:Nullable<int>;mutable UserId:int;mutable IsComplete:bool;mutable ChildTaskCount:int option}


type MeatGrinderEntities() = 
    inherit DbContext("name=MeatGrinderEntities")
//    override x.OnModelCreating (modelBuilder:DbModelBuilder) =
//        ()
    let mutable _goals:DbSet<Goal> = null
    let mutable _users:DbSet<User> = null
    let mutable _tasks:DbSet<Task> = null
    member public x.Goals 
        with get() = _goals
        and  set v = _goals <- v
    member public x.Users
        with get() = _users
        and set v = _users <- v
    member public x.Tasks
        with get() = _tasks
        and set v = _tasks <- v

type GoalViewModel = {Goals:Goal list;}
type TaskViewModel = {BreadCrumbs: BreadCrumbModel list; Tasks:Task list}