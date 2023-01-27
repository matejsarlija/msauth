var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataProtection();

var app = builder.Build();


app.MapGet("/user", (HttpContext ctx) =>
{
    var authCookie = ctx.Request.Headers.Cookie.FirstOrDefault(x => x.StartsWith("auth="));
    var payload = authCookie.Split("=").Last();
    var parts = payload.Split(":");
    var key = parts[0];
    var value = parts[1];
    return key + value;

});

app.MapGet("/login", (HttpContext ctx) =>
{
    ctx.Response.Headers["set-cookie"] = "auth=user:me";
    return "ok";
});

app.Run();
