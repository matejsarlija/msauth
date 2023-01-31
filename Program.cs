using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication()
    .AddCookie("visitor")
    .AddCookie("local");

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

public class VisitorAuthHandler : CookieAuthenticationHandler
{
    public VisitorAuthHandler(
        IOptionsMonitor<CookieAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var result = await base.HandleAuthenticateAsync();

        if (result.Succeeded)
        {
            return result;
        }
    }
}
