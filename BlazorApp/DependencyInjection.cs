using BlazorApp.Interfaces;
using BlazorApp.Persistence;
using BlazorApp.Persistence.Repositories;
using BlazorApp.Shared.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Radzen;
using Duende.IdentityServer.Models;
using DClient = Duende.IdentityServer.Models.Client;
using DIdentityResources = Duende.IdentityServer.Models.IdentityResources;
using Duende.IdentityServer.Test;

namespace BlazorApp;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddHttpClient();
        services.AddControllers();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddPersistence(configuration);
        services.ConfigureAuthentication(configuration);

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ICustomerService, CustomerService>();

        services.AddScoped<ITokenService, TokenService>();

        services.AddRadzenComponents();

        return services;
    }

    public static IServiceCollection AddPersistence(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddScoped<ApplicationDbContextInitialiser>();

        services.AddDbContext<ApplicationDbContext>(
            (sp, options) =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection")
                    ?? throw new ArgumentNullException("DefaultConnection");

                options.UseSqlServer(connectionString).AddAsyncSeeding(sp);
            });

        return services;
    }

    public static IServiceCollection ConfigureAuthentication(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddIdentityServer()
            .AddDeveloperSigningCredential()
            .AddInMemoryClients(IdentityServerConfig.GetClients())
            .AddInMemoryApiScopes(IdentityServerConfig.GetApiScopes())
            .AddInMemoryApiResources(IdentityServerConfig.GetApiResources())
            .AddTestUsers(IdentityServerConfig.GetTestUsers());

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = "https://localhost:7018";  // The issuer
                options.Audience = "api";  // The intended audience
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    // ClockSkew = TimeSpan.Zero  // No tolerance for expiration time
                };
            });

        services.AddAuthorization();

        return services;
    }

}
public static class IdentityServerConfig
{
    public static IEnumerable<DClient> GetClients() =>
    [
        new DClient
        {
            ClientId = "m2m",
            ClientName = "Machine to machine (client credentials)",
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            ClientSecrets = { new Secret("secret".Sha256()) },
            AllowedScopes = { "api" },
        }
    ];

    public static IEnumerable<ApiScope> GetApiScopes() =>
    [
        new ApiScope("api", "Access to API")
    ];

    public static IEnumerable<ApiResource> GetApiResources() =>
    [
        new ApiResource("api", "BlazorApp API")
        {
            Scopes = { "api" }
        }
    ];

    public static List<TestUser> GetTestUsers() =>
    [
        new TestUser
        {
            SubjectId = "1",
            Username = "user",
            Password = "password"
        }
    ];
}

