using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);


// request / resp go through Services
builder.Services.AddDataProtection();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthService>();

var app = builder.Build();

// then go through middleware
app.Use((ctx, next) =>
{
    var idp = ctx.RequestServices.GetRequiredService<IDataProtectionProvider>();
    var protector = idp.CreateProtector("auth-cookie");

    var authCookie = ctx.Request.Headers.Cookie.FirstOrDefault(x => x.StartsWith("auth="));
    var protectedPayload = authCookie.Split("=").Last();
    var payload = protector.Unprotect(protectedPayload);
    var parts = payload.Split(":");
    var key = parts[0];
    var value = parts[1];


    return next();

});

app.MapGet("/user", (HttpContext ctx, IDataProtectionProvider idp) =>
{


    return value;

});

app.MapGet("/login", (HttpContext ctx) =>
{
    return ctx.User;
});

app.Run();

public class AuthService
{
    private readonly IDataProtectionProvider _idp;
    private readonly IHttpContextAccessor _accessor;

    public AuthService(IDataProtectionProvider idp, IHttpContextAccessor accessor)
    {
        _idp = idp;
        _accessor = accessor;
    }

    public void SignIn()
    {
        var protector = _idp.CreateProtector("auth-cookie");

        _accessor.HttpContext.Response.Headers["set-cookie"] = $"auth={protector.Protect("usr:me")}";
    }
}
