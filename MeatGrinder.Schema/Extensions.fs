[<AutoOpen>]
module Extensions
open System
// http://stackoverflow.com/questions/1523500/how-do-i-create-an-extension-method-f
type System.String with
    member x.IsNullOrEmpty () =
        System.String.IsNullOrEmpty(x)
// http://stackoverflow.com/questions/15145009/concise-notation-for-last-element-of-an-array
//type System.Collections.Generic.IList<'T> with
//    member x.Last = 
//        x.[x.Count-1]
