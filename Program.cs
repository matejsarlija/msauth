using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication().AddCookie("local");

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", ctx => Task.FromResult("Hello World!"));

app.MapGet("/login", async (HttpContext ctx) =>
{
    var claims = new List<Claim>();
    claims.Add(new Claim("usr", "me"));
    var identity = new ClaimsIdentity(claims, "cookie");
    var user = new ClaimsPrincipal(identity);

    await ctx.SignInAsync("local", user);

});

app.Run();

// public class AuthService
// {
//     private readonly IDataProtectionProvider _idp;
//     private readonly IHttpContextAccessor _accessor;

//     public AuthService(IDataProtectionProvider idp, IHttpContextAccessor accessor)
//     {
//         _idp = idp;
//         _accessor = accessor;
//     }

//     public void SignIn()
//     {
//         var protector = _idp.CreateProtector("auth-cookie");
//         _accessor.HttpContext.Response.Headers["set-cookie"] = $"auth={protector.Protect("usr:me")}";
//     }
// }


public class MyRequirement : IAuthorizationRequirement
{

}

public class MyRequirementHandler : AuthorizationHandler<MyRequirement>
{
    public MyRequirementHandler()
    {

    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MyRequirement requirement)
    {

        // context.User
        //context.Succeed(new MyRequirement());
        return Task.CompletedTask;
    }
}
