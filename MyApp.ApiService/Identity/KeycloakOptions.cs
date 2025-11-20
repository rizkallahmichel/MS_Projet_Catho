namespace MyApp.ApiService.Identity;

public sealed class KeycloakOptions
{
    public string BaseUrl { get; set; } = string.Empty;
    public string Realm { get; set; } = string.Empty;
    public string AdminRealm { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string? ClientSecret { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string PharmacyRoleName { get; set; } = "pharmacy";
    public string ClientRoleName { get; set; } = "client";
}
