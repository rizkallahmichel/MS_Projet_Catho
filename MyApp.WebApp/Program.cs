using System;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.IdentityModel.Tokens;
using MyApp.WebApp.Authentication;
using MyApp.WebApp.Clients;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
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
    options.Scope.Add("profile");

    options.ClaimActions.MapJsonKey("preferred_username", "preferred_username");
    options.ClaimActions.MapJsonSubKey("role", "realm_access", "roles");
    options.ClaimActions.MapJsonSubKey("role", "resource_access", "roles");

    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = "name",
        RoleClaimType = "role"
    };
});

builder.Services.ConfigureCookieOidc(CookieAuthenticationDefaults.AuthenticationScheme, "oidc");
builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<ApiTokenHandler>();

var apiBaseAddress = builder.Configuration["Api:BaseAddress"];
if (string.IsNullOrWhiteSpace(apiBaseAddress))
{
    apiBaseAddress = "https+http://apiservice";
}

builder.Services.AddHttpClient<ICmsApiClient, CmsApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseAddress);
}).AddHttpMessageHandler<ApiTokenHandler>();

var app = builder.Build();

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

app.MapGet("/authentication/login", async (HttpContext context) =>
{
    var returnUrl = context.Request.Query["returnUrl"].FirstOrDefault();
    returnUrl = string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl;

    await context.ChallengeAsync("oidc", new AuthenticationProperties
    {
        RedirectUri = returnUrl
    });
}).AllowAnonymous();

app.MapGet("/authentication/logout", async (HttpContext context) =>
{
    var returnUrl = context.Request.Query["returnUrl"].FirstOrDefault() ?? "/";

    var authResult = await context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    var properties = authResult?.Properties ?? new AuthenticationProperties();
    properties.RedirectUri = returnUrl;

    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

    if (authResult?.Principal?.Identity?.IsAuthenticated == true)
    {
        await context.SignOutAsync("oidc", properties);
    }
    else
    {
        context.Response.Redirect(returnUrl);
    }
}).AllowAnonymous();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
