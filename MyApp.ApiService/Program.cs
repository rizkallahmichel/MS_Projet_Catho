using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyApp.ApiService.Middlewares;
using MyApp.ApiService.Identity;
using MyApp.Application;
using MyApp.Application.Common.Interfaces;
using MyApp.Persistence;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.AddSqlServerDbContext<MyAppDbContext>(connectionName: "myapp");

builder.Services
    .AddApplication()
    .AddPersistenceLayer();

builder.Services.Configure<KeycloakOptions>(builder.Configuration.GetSection("Keycloak"));

builder.Services.AddHttpClient<IIdentityProvisioningService, KeycloakIdentityProvisioningService>((serviceProvider, client) =>
{
    var options = serviceProvider.GetRequiredService<IOptions<KeycloakOptions>>().Value;
    if (!string.IsNullOrWhiteSpace(options.BaseUrl))
    {
        var baseAddress = options.BaseUrl.EndsWith('/') ? options.BaseUrl : $"{options.BaseUrl}/";
        client.BaseAddress = new Uri(baseAddress);
    }
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.Authority = builder.Configuration["Authentication:OIDC:Authority"];
        options.Audience = builder.Configuration["Authentication:OIDC:Audience"];
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
        };
        options.MapInboundClaims = false;
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                if (context.Principal?.Identity is not ClaimsIdentity identity)
                {
                    return Task.CompletedTask;
                }

                var roles = ResolveRoles(identity.Claims);
                foreach (var role in roles)
                {
                    if (!identity.HasClaim(identity.RoleClaimType, role))
                    {
                        identity.AddClaim(new Claim(identity.RoleClaimType, role));
                    }
                }

                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogError(context.Exception, "JWT authentication failed for {Path}", context.HttpContext.Request.Path);
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogWarning(
                    "JWT challenge for {Path}. Error: {Error}. Description: {Description}",
                    context.HttpContext.Request.Path,
                    context.Error,
                    context.ErrorDescription);
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy =>
        policy.RequireAssertion(context =>
        {
            var roles = ResolveRoles(context.User.Claims);
            return roles.Contains("admin", StringComparer.OrdinalIgnoreCase);
        }));

    options.AddPolicy("RequireAdminOrPharmacyRole", policy =>
        policy.RequireAssertion(context =>
        {
            var roles = ResolveRoles(context.User.Claims);
            return roles.Contains("admin", StringComparer.OrdinalIgnoreCase) ||
                   roles.Contains("pharmacy", StringComparer.OrdinalIgnoreCase);
        }));

    options.AddPolicy("RequireClientRole", policy =>
        policy.RequireAssertion(context =>
        {
            var roles = ResolveRoles(context.User.Claims);
            return roles.Contains("client", StringComparer.OrdinalIgnoreCase);
        }));
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "MyApp API", Version = "v1" });

    var authority = builder.Configuration["Authentication:OIDC:Authority"];
    if (!string.IsNullOrWhiteSpace(authority))
    {
        var trimmedAuthority = authority.TrimEnd('/');
        var authorizationUrl = new Uri($"{trimmedAuthority}/protocol/openid-connect/auth");
        var tokenUrl = new Uri($"{trimmedAuthority}/protocol/openid-connect/token");

        var configuredScopes = builder.Configuration
            .GetSection("SwaggerOAuth:Scopes")
            .GetChildren()
            .Where(scope => !string.IsNullOrWhiteSpace(scope.Key))
            .ToDictionary(
                scope => scope.Key!,
                scope => string.IsNullOrWhiteSpace(scope.Value) ? scope.Key! : scope.Value!);

        if (configuredScopes.Count == 0)
        {
            configuredScopes = new Dictionary<string, string>
            {
                ["openid"] = "OpenID scope",
                ["profile"] = "User profile",
                ["api"] = "Access MyApp API"
            };
        }

        options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = authorizationUrl,
                    TokenUrl = tokenUrl,
                    Scopes = configuredScopes
                }
            }
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "oauth2"
                    }
                },
                configuredScopes.Keys.ToArray()
            }
        });
    }
});

var app = builder.Build();
app.MapDefaultEndpoints();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MyAppDbContext>();
    await db.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "MyApp API v1");

        var swaggerOAuthSection = builder.Configuration.GetSection("SwaggerOAuth");
        var clientId = swaggerOAuthSection["ClientId"];
        var clientSecret = swaggerOAuthSection["ClientSecret"];
        var scopes = swaggerOAuthSection
            .GetSection("Scopes")
            .GetChildren()
            .Select(scope => scope.Key)
            .Where(key => !string.IsNullOrWhiteSpace(key))
            .ToArray();

        if (scopes.Length == 0)
        {
            scopes = new[] { "openid", "profile", "api" };
        }

        if (!string.IsNullOrWhiteSpace(clientId))
        {
            options.OAuthClientId(clientId);
        }

        if (!string.IsNullOrWhiteSpace(clientSecret))
        {
            options.OAuthClientSecret(clientSecret);
        }

        options.OAuthScopes(scopes);
        options.OAuthUsePkce();
    });
}

app.UseApiExceptionHandling();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

static HashSet<string> ResolveRoles(IEnumerable<Claim> claims)
{
    var claimList = claims.ToList();
    var roles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    foreach (var claim in claimList)
    {
        if (!string.IsNullOrWhiteSpace(claim.Value) &&
            (string.Equals(claim.Type, ClaimTypes.Role, StringComparison.OrdinalIgnoreCase) ||
             string.Equals(claim.Type, "role", StringComparison.OrdinalIgnoreCase)))
        {
            roles.Add(claim.Value);
        }
    }

    MergeRolesFromRealmAccess(claimList.FirstOrDefault(c => c.Type == "realm_access")?.Value, roles);
    MergeRolesFromResourceAccess(claimList.FirstOrDefault(c => c.Type == "resource_access")?.Value, roles);

    return roles;
}

static void MergeRolesFromRealmAccess(string? json, HashSet<string> roles)
{
    if (string.IsNullOrWhiteSpace(json))
    {
        return;
    }

    try
    {
        using var doc = JsonDocument.Parse(json);
        if (doc.RootElement.TryGetProperty("roles", out var rolesArray))
        {
            foreach (var item in rolesArray.EnumerateArray())
            {
                var value = item.GetString();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    roles.Add(value);
                }
            }
        }
    }
    catch
    {
        // ignore parsing errors
    }
}

static void MergeRolesFromResourceAccess(string? json, HashSet<string> roles)
{
    if (string.IsNullOrWhiteSpace(json))
    {
        return;
    }

    try
    {
        using var doc = JsonDocument.Parse(json);
        foreach (var resource in doc.RootElement.EnumerateObject())
        {
            if (resource.Value.TryGetProperty("roles", out var rolesArray))
            {
                foreach (var item in rolesArray.EnumerateArray())
                {
                    var value = item.GetString();
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        roles.Add(value);
                    }
                }
            }
        }
    }
    catch
    {
        // ignore parsing errors
    }
}
