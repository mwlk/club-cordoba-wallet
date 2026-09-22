using ClubCordobaWallet.Infrastructure.Persistence;
using ClubCordobaWallet.Infrastructure.Repositories;
using ClubCordobaWallet.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClubCordobaWallet.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Default")));

        services.AddScoped<IMemberRepository, MemberRepository>();
        services.AddScoped<ICredentialRepository, CredentialRepository>();
        services.AddScoped<IIssuerService, IssuerService>();
        services.AddScoped<ITenantService, TenantService>();

        return services;
    }
}
