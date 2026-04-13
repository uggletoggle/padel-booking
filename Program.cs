using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using PadelBooking.Api.Data.Enums;
using PadelBooking.Api.Mappings;
using PadelBooking.Api.Endpoints;
using PadelBooking.Api.Validators.Clubs;
using PadelBooking.Api.Security;
using PadelBooking.Api.Services;
using PadelBooking.Api.Data.Repositories;
using PadelBooking.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();

var keycloakAuthority = builder.Configuration["Keycloak:Authority"]!;
var keycloakAudience = builder.Configuration["Keycloak:Audience"]!;

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
    
// Registering the generic repository
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PadelBooking API", Version = "v1" });

    // OAuth2 / Keycloak security scheme
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            // Password flow so you can type username/password directly in Swagger UI
            Password = new OpenApiOAuthFlow
            {
                TokenUrl = new Uri($"{keycloakAuthority}/protocol/openid-connect/token"),
                AuthorizationUrl = new Uri($"{keycloakAuthority}/protocol/openid-connect/auth"),
                Scopes = new Dictionary<string, string>
                {
                    { "openid", "OpenID Connect" },
                    { "profile", "Profile" },
                }
            },
            // Authorization Code + PKCE — use this for the full browser redirect flow
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"{keycloakAuthority}/protocol/openid-connect/auth"),
                TokenUrl = new Uri($"{keycloakAuthority}/protocol/openid-connect/token"),
                Scopes = new Dictionary<string, string>
                {
                    { "openid",  "OpenID Connect" },
                    { "profile", "Profile" },
                }
            }
        }
    });

    // Attach the Bearer token to every secured operation
    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("oauth2", document)] = new List<string> { "openid", "profile" }
    });
});

builder.Services.AddOpenApi();

// Register FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateClubRequestValidator>();

// Security Services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContext, UserContext>();
builder.Services.AddTransient<IClaimsTransformation, KeycloakClaimsTransformation>();

// Register typed HttpClient for KeycloakAdminService with Resilience (Retries, Timeouts)
builder.Services.AddHttpClient<KeycloakAdminService>()
    .AddStandardResilienceHandler();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Keycloak:Authority"];
        options.Audience = builder.Configuration["Keycloak:Audience"];
        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false, // Set to false to avoid issuer mismatch issues during initial setup
            NameClaimType = "preferred_username"
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogError("Authentication failed: {Message}", context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("Token validated for user: {User}", context.Principal?.Identity?.Name);
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RequireAdminRole", policy => policy.RequireRole(UserRole.Admin.ToString()))
    .AddPolicy("RequireOwnerRole", policy => policy.RequireRole(UserRole.Owner.ToString()))
    .AddPolicy("RequireAttendantRole", policy => policy.RequireRole(UserRole.Attendant.ToString()));

// Register Mapster
TypeAdapterConfig.GlobalSettings.Scan(typeof(MappingConfig).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(ui =>
    {
        ui.SwaggerEndpoint("/swagger/v1/swagger.json", "PadelBooking API v1");

        // OAuth2 client settings — set your Keycloak client ID here
        ui.OAuthClientId(keycloakAudience);
        ui.OAuthClientSecret(string.Empty); // leave empty if it's a public client
        ui.OAuthRealm("padelbooking");
        ui.OAuthAppName("PadelBooking Swagger UI");
        ui.OAuthUsePkce();       // enables PKCE for the auth-code flow
        ui.OAuthScopes("openid", "profile");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Map Minimal API Endpoints
// Apply .RequireAuthorization() globally to mapped groups
app.MapGroup("/api/clubs").MapClubEndpoints().WithTags("Clubs").RequireAuthorization();
app.MapGroup("/api/courts").MapCourtEndpoints().WithTags("Courts").RequireAuthorization();
app.MapGroup("/api/fixed-reservations").MapFixedReservationEndpoints().WithTags("FixedReservations").RequireAuthorization();
app.MapGroup("/api/reservations").MapReservationEndpoints().WithTags("Reservations").RequireAuthorization();
app.MapGroup("/api/users").MapUserEndpoints().WithTags("Users");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
