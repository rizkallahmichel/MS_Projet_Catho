using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.IdentityModel.Tokens;
using MyApp.WebApp.Authentication;
using MyApp.WebApp.Clients;

var builder = WebApplication.CreateBuilder(args);

// --------------------
// UI & Core Setup
// --------------------
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.AddServiceDefaults();

// --------------------
// Authentication
// --------------------
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

    // Requested scopes
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");
    options.Scope.Add("api");

    // Claim mapping
    options.ClaimActions.MapJsonKey(ClaimTypes.Name, "preferred_username");
    options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
    options.ClaimActions.MapJsonKey("preferred_username", "preferred_username");
    options.ClaimActions.MapJsonKey("realm_access", "realm_access");
    options.ClaimActions.MapJsonKey("resource_access", "resource_access");

    // Ensure .NET knows what the "role" claim is
    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = ClaimTypes.Name,
        RoleClaimType = ClaimTypes.Role
    };

    // Merge Keycloak roles into .NET roles
    options.Events = new OpenIdConnectEvents
    {
        OnTokenValidated = context =>
        {
            var identity = (ClaimsIdentity)context.Principal.Identity;

            var allRoles = new List<string>();

            // Extract realm_access.roles
            var realmAccessClaim = context.Principal.FindFirst("realm_access")?.Value;
            if (!string.IsNullOrEmpty(realmAccessClaim))
            {
                try
                {
                    using var doc = JsonDocument.Parse(realmAccessClaim);
                    if (doc.RootElement.TryGetProperty("roles", out var rolesArray))
                    {
                        foreach (var role in rolesArray.EnumerateArray())
                        {
                            allRoles.Add(role.GetString());
                        }
                    }
                }
                catch { }
            }

            // Extract resource_access.*.roles
            var resourceAccessClaim = context.Principal.FindFirst("resource_access")?.Value;
            if (!string.IsNullOrEmpty(resourceAccessClaim))
            {
                try
                {
                    using var doc = JsonDocument.Parse(resourceAccessClaim);
                    foreach (var resource in doc.RootElement.EnumerateObject())
                    {
                        if (resource.Value.TryGetProperty("roles", out var rolesArray))
                        {
                            foreach (var role in rolesArray.EnumerateArray())
                            {
                                allRoles.Add(role.GetString());
                            }
                        }
                    }
                }
                catch { }
            }

            // Add all roles as ClaimTypes.Role
            foreach (var role in allRoles.Distinct())
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            return Task.CompletedTask;
        }
    };
});

// --------------------
// Authorization
// --------------------
builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.ConfigureCookieOidc(CookieAuthenticationDefaults.AuthenticationScheme, "oidc");

// --------------------
// API Client Setup
// --------------------
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

builder.Services.AddHttpClient<IIdentityApiClient, IdentityApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseAddress);
});

// --------------------
// Build App
// --------------------
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

// --------------------
// Authentication Endpoints
// --------------------
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

// --------------------
// Blazor Mapping
// --------------------
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
