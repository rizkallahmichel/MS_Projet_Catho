using System;
using Microsoft.AspNetCore.Authentication;
using System.Linq;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using MyApp.WebApp.Authentication;
using MyApp.WebApp.Clients;
using MyApp.WebApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.AddServiceDefaults();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "oidc";
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
.AddOpenIdConnect("oidc", options =>
{
    options.Authority = builder.Configuration["Authentication:OIDC:Authority"];
    options.ClientId = builder.Configuration["Authentication:OIDC:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:OIDC:ClientSecret"];
    options.RequireHttpsMetadata = false;
    options.ResponseType = "code";
    options.SaveTokens = true;
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.CallbackPath = "/signin-oidc";
    options.SignedOutCallbackPath = "/signout-callback-oidc";
    options.UseTokenLifetime = true;
    options.MapInboundClaims = false;
    options.Scope.Add("api");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = "name",
        RoleClaimType = "role",
    };
});
builder.Services.ConfigureCookieOidc(CookieAuthenticationDefaults.AuthenticationScheme, "oidc");
builder.Services.AddAuthorization();
builder.Services.AddAntiforgery();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<ApiTokenHandler>();
builder.Services.AddHttpClient<ITodoClient, TodoClient>(client =>
{
    client.BaseAddress = new Uri("https+http://apiservice");
}).AddHttpMessageHandler<ApiTokenHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapGet("/authentication/login", async (HttpContext context) =>
{
    var returnUrl = context.Request.Query["returnUrl"].FirstOrDefault();
    if (string.IsNullOrEmpty(returnUrl))
    {
        returnUrl = "/";
    }

    await context.ChallengeAsync("oidc", new AuthenticationProperties
    {
        RedirectUri = returnUrl
    });
}).AllowAnonymous();

app.MapGet("/authentication/logout", async (HttpContext context) =>
{
    var returnUrl = context.Request.Query["returnUrl"].FirstOrDefault() ?? "/";

    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    await context.SignOutAsync("oidc", new AuthenticationProperties
    {
        RedirectUri = returnUrl
    });
}).AllowAnonymous();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
