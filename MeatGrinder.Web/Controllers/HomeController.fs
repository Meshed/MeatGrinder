namespace MeatGrinder.Web.Controllers

open System
open System.Collections.Generic
open System.Globalization
open System.Security.Cryptography
open System.Text
open System.Linq
open System.Web
open System.Web.Mvc
open System.Web.Mvc.Ajax

open Extensions

open MeatGrinder.DAL.Models
open MeatGrinder.Web.Helpers
open MeatGrinder.Web.Services

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

    member this.Required (value:string) (msg:string) = 
        if value.IsNullOrEmpty() then this.ModelState.AddModelError(String.Empty, msg)

    [<CustomAuthorize>]
    member this.Index () = this.View()

    [<CustomAuthorize>]
    member this.AccountDetails () = 
        let account = new MeatGrinder.DAL.Models.User (AccountName="Meshed",EmailAddress="marksbrown@gmail.com")
        this.Json(account,JsonRequestBehavior.AllowGet)

    [<HttpGet>]
    member this.Login () = this.View()
    
    [<HttpPost>]
    member this.Login (viewModel:LoginViewModel) :ActionResult =
        this.Required viewModel.Password "Password is required!"
        this.Required viewModel.EmailAddress "Email address is required!"
        if not this.ModelState.IsValid then
            this.View(viewModel) :> ActionResult
        else
            let passwordHash = GetHash(viewModel.Password)
            use db = new MeatGrinderEntities()
            let user = db.Users.FirstOrDefault(fun i -> i.Password = passwordHash && i.EmailAddress = viewModel.EmailAddress)
            if user <> null then
                CookieService.SetCookie(this.Response, CookieService.userId, 1, user.ID.ToString(CultureInfo.InvariantCulture))
                this.RedirectToAction("Index") :> ActionResult
            else
                this.ModelState.AddModelError("", "Invalid email or password!");
                this.View(viewModel) :> ActionResult
    [<HttpGet>]
    member this.LogOff () = 
        CookieService.DeleteCookie this.Response CookieService.userId
        this.RedirectToAction("Index")

    [<HttpGet>]
    member this.Register () = this.View()
    [<HttpPost>]
    member this.Register (viewModel:LoginViewModel) :ActionResult =
        this.Required viewModel.EmailAddress "Email address is required!"
        this.Required viewModel.AccountName "Account Name is required!"
        this.Required viewModel.Password "Password is required!"
        let toLogin (msg:string) view =
            this.ModelState.AddModelError(System.String.Empty,msg)
            view("Login", viewModel) :> ActionResult
        if this.ModelState.IsValid then
            use db = new MeatGrinderEntities()
            match db.Users.FirstOrDefault( fun m -> m.AccountName=viewModel.AccountName || m.EmailAddress=viewModel.EmailAddress) 
                with
                | acc when acc<>null && acc.AccountName=viewModel.AccountName -> 
                    toLogin "Account name already exists. Please try again." this.View
                | em when em<> null && em.EmailAddress=viewModel.EmailAddress ->
                    toLogin "Email address already exists. Please try again." this.View
                | _ -> 
                    let user = User(AccountName=viewModel.AccountName, EmailAddress=viewModel.EmailAddress, Password = GetHash(viewModel.Password),
                                    DateCreated = Nullable(DateTime.Now))
                    db.Users.Add(user) |> ignore
                    db.SaveChanges() |> ignore
                    CookieService.SetCookie(this.Response, CookieService.userId, 1, user.ID.ToString(CultureInfo.InvariantCulture));
                    this.RedirectToAction("Index") :> ActionResult
        else 
            this.View("Login", viewModel) :> ActionResult
    [<HttpGet>]
    member this.Landing () = new FilePathResult("/Views/Home/landing.html", "text/html")