using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Configure forwarded headers for reverse proxy (Caddy)
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<PadelBookingAdmin.Services.PadelApiService>();


builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    var keycloakConfig = builder.Configuration.GetSection("Keycloak");
    options.Authority = keycloakConfig["Authority"];
    options.ClientId = keycloakConfig["ClientId"];
    options.ClientSecret = keycloakConfig["ClientSecret"];
    options.ResponseType = "code";
    options.SaveTokens = true;
    
    // Add scopes matching the Keycloak setup
    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");

    options.RequireHttpsMetadata = keycloakConfig.GetValue<bool>("RequireHttpsMetadata", true);

    // Use backchannel URL for token/metadata requests inside Docker network
    var metadataAddress = keycloakConfig["MetadataAddress"];
    if (!string.IsNullOrEmpty(metadataAddress))
    {
        options.MetadataAddress = metadataAddress;
    }
    
    // Disable Pushed Authorization Requests (PAR) — Keycloak 26+ enables it by default
    // but the .NET OIDC handler doesn't support it natively
    options.PushedAuthorizationBehavior = PushedAuthorizationBehavior.Disable;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = "preferred_username",
        RoleClaimType = "roles",
        ValidIssuer = keycloakConfig["Authority"]
    };
});

// Configure base HTTP Client for the backend API
builder.Services.AddHttpClient("PadelApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiConfigs:PadelApi"]!);
});

var app = builder.Build();

// Must be first middleware — applies forwarded headers from reverse proxy
app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
app.UseExceptionHandler("/Home/Error");

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
