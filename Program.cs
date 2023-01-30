using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using System.Security.Claims;

const string AuthScheme = "cookie";
const string AuthScheme2 = "cookie2";


var builder = WebApplication.CreateBuilder(args);


// request / resp go through Services
//builder.Services.AddDataProtection();
//builder.Services.AddHttpContextAccessor();
//builder.Services.AddScoped<AuthService>();

builder.Services.AddAuthentication(AuthScheme).AddCookie(AuthScheme).AddCookie(AuthScheme2);

var app = builder.Build();

// then go through middleware
// app.Use((ctx, next) =>
// {
//     var idp = ctx.RequestServices.GetRequiredService<IDataProtectionProvider>();
//     var protector = idp.CreateProtector("auth-cookie");

//     var authCookie = ctx.Request.Headers.Cookie.FirstOrDefault(x => x.StartsWith("auth="));
//     var protectedPayload = authCookie.Split("=").Last();
//     var payload = protector.Unprotect(protectedPayload);
//     var parts = payload.Split(":");
//     var key = parts[0];
//     var value = parts[1];

//     var claims = new List<Claim>();
//     claims.Add(new Claim(key, value));
//     var identity = new ClaimsIdentity();
//     ctx.User = new ClaimsPrincipal(identity);


//     return next();

// });

app.UseAuthentication();

app.Use((ctx, next) =>
{
    if (ctx.Request.Path.StartsWithSegments("/login"))
    {
        return next();
    }

    if (!ctx.User.Identities.Any(x => x.AuthenticationType == AuthScheme))
    {
        ctx.Response.StatusCode = 401;
        return Task.CompletedTask;

    }

    if (!ctx.User.HasClaim("passport_type", "eu"))
    {
        ctx.Response.StatusCode= 403;

        return Task.CompletedTask;
    }

    return next();

});

app.MapGet("/user", (HttpContext ctx) =>
{

    return ctx.User.FindFirst("usr")?.Value ?? "empty";

});

app.MapGet("/sweden", (HttpContext ctx) =>
{
    // if (!ctx.User.Identities.Any(x => x.AuthenticationType == AuthScheme))
    // {
    //     ctx.Response.StatusCode = 401;
    //     return "";

    // }

    // if (!ctx.User.HasClaim("passport_type", "eu"))
    // {
    //     ctx.Response.StatusCode= 403;

    //     return "not allowed";
    // }

    return "allowed";

});

app.MapGet("/norway", (HttpContext ctx) =>
{
    // if (!ctx.User.Identities.Any(x => x.AuthenticationType == AuthScheme))
    // {
    //     ctx.Response.StatusCode = 401;
    //     return "";

    // }

    // if (!ctx.User.HasClaim("passport_type", "NOR"))
    // {
    //     ctx.Response.StatusCode= 403;

    //     return "not allowed";
    // }

    return "allowed";

});


// [AuthScheme(AuthScheme2)]
app.MapGet("/denmark", (HttpContext ctx) =>
{
    if (!ctx.User.Identities.Any(x => x.AuthenticationType == AuthScheme2))
    {
        ctx.Response.StatusCode = 401;
        return "";

    }

    if (!ctx.User.HasClaim("passport_type", "eu"))
    {
        ctx.Response.StatusCode= 403;

        return "not allowed";
    }

    return "allowed";

});

app.MapGet("/login", async (HttpContext ctx) =>
{
    //auth.SignIn();

    var claims = new List<Claim>();
    claims.Add(new Claim("usr", "me"));
    claims.Add(new Claim("passport_type", "eu"));
    var identity = new ClaimsIdentity(claims, AuthScheme);
    var user = new ClaimsPrincipal(identity);
    await ctx.SignInAsync(AuthScheme, user);
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
