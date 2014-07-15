namespace MeatGrinder.Web.Controllers

open System
open System.Text
open System.Collections.Generic
open System.Linq
open System.Web
open System.Web.Mvc
open System.Web.Mvc.Ajax

open Extensions

open System.Security.Cryptography

open MeatGrinder.DAL.Models
open MeatGrinder.Web.Helpers


type HomeController() =
    inherit Controller()
    let GetHash (password:string) =
        let md5Hash = MD5.Create()

        // Convert the input string to a byte array and compute the hash. 
        //let encoded = 
        let data = md5Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password))

        // Create a new Stringbuilder to collect the bytes 
        // and create a string.
        let sBuilder = new StringBuilder();

        // Loop through each byte of the hashed data  
        // and format each one as a hexadecimal string. 
        for i in 1..data.Length do
            let string = data.[i].ToString("x2")
            sBuilder.Append(string) |> ignore
        
        // Return the hexadecimal string. 
        sBuilder.ToString()
        
    [<CustomAuthorize>]
    member this.Index () = this.View()
    [<CustomAuthorize>]
    member this.AccountDetails () = 
        let account = new MeatGrinder.DAL.Models.User (AccountName="Meshed",EmailAddress="marksbrown@gmail.com")
        this.Json(account,JsonRequestBehavior.AllowGet)
    [<HttpGet>]
    member this.Login () = this.View()
    
    [<HttpPost>]
    member this.Login (viewModel:LoginViewModel) =
        if viewModel.Password.IsNullOrEmpty() then this.ModelState.AddModelError(System.String.Empty, "Password is required!")
        if viewModel.EmailAddress.IsNullOrEmpty() then this.ModelState.AddModelError("", "Email address is required!")
        if not this.ModelState.IsValid then
            this.View(viewModel)
        else
            let passwordHash = GetHash(viewModel.Password)
            let db = new MeatGrinderEntities()
            let user = db.Users.FirstOrDefault(fun i -> i.Password = passwordHash && i.EmailAddress = viewModel.EmailAddress)
            this.View(viewModel)
