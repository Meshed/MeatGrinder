module DbContext
open Microsoft.FSharp.Data.TypeProviders
type MeatGrinderContext = SqlDataConnection<""" Server=tcp:ywckntoq5w.database.windows.net,1433;Database=MeatGrinder;User ID=meat_grinder@ywckntoq5w;Password=Shoestrings2;MultipleActiveResultSets=True;Encrypt=True;Connection Timeout=30 """>