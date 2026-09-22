using ClubCordobaWallet.Application.Credentials.Commands.CreateCredential;
using ClubCordobaWallet.Application.Credentials.Dtos;
using ClubCordobaWallet.Application.Credentials.Queries.GetCredentialById;
using ClubCordobaWallet.Application.Credentials.Queries.GetCredentials;
using ClubCordobaWallet.Application.Credentials.Queries.SearchMemberByDni;
using Microsoft.Extensions.DependencyInjection;

namespace ClubCordobaWallet.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<CreateCredentialCommand, Result<CreateCredentialResult>>,
            CreateCredentialCommandHandler>();

        services.AddScoped<IQueryHandler<SearchMemberByDniQuery, Result<MemberSearchDto>>,
            SearchMemberByDniQueryHandler>();

        services.AddScoped<IQueryHandler<GetCredentialsQuery, List<CredentialListDto>>,
            GetCredentialsQueryHandler>();

        services.AddScoped<IQueryHandler<GetCredentialByIdQuery, Result<CredentialDetailDto>>,
            GetCredentialByIdQueryHandler>();

        return services;
    }
}
