module Extensions
open System
// http://stackoverflow.com/questions/1523500/how-do-i-create-an-extension-method-f
type System.String with
    member x.IsNullOrEmpty () =
        System.String.IsNullOrEmpty(x)

