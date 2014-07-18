namespace MeatGrinder.Schema

open System.Collections.Generic

type AdminSummaryViewModel = {TotalUserCount:int}
type BreadCrumbModel = {DisplayName:string;Url:string}
type TodoViewModel = {Id:int;Description:string;TaskType:string;GoalName:string;ParentTaskName:string}
type BreadCrumbWrapper (breadCrumbs:List<BreadCrumbModel>) =
    member x.BreadCrumbs = if breadCrumbs<>null then breadCrumbs else List<BreadCrumbModel>()
