namespace MeatGrinder.Orm
open System
open System.Data.Entity
open System.Data.Entity.Infrastructure
open System.Data.Objects
open System.Data.Objects.DataClasses
open System.Linq
type Goal = {Id:int}

type MeatGrinderEntities() = 
    inherit DbContext("name=MeatGrinderEntities")
//    override x.OnModelCreating (modelBuilder:DbModelBuilder) =
//        ()
    let mutable _goals:DbSet<Goal> = null
    member public x.Goals 
        with get() = _goals
        and  set v = _goals <- v
