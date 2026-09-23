using ClubCordobaWallet.Application.Features.Credentials.Queries.GetCredentials;
using ClubCordobaWallet.Application.Features.Credentials.Queries.SearchMemberByDni;
using ClubCordobaWallet.Application.Features.Credentials.Commands.CreateCredential;
using ClubCordobaWallet.Application.Features.Credentials.Queries.GetCredentialById;
using Microsoft.Extensions.DependencyInjection;
using ClubCordobaWallet.Application.Features.Credentials.Dtos;

namespace ClubCordobaWallet.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<CreateCredentialCommand, Result<CreateCredentialResult>>,
            CreateCredentialCommandHandler>();

        services.AddScoped<IQueryHandler<SearchMemberByDniQuery, Result<List<MemberSearchDto>>>,
            SearchMemberByDniQueryHandler>();

        services.AddScoped<IQueryHandler<GetCredentialsQuery, List<CredentialListDto>>,
            GetCredentialsQueryHandler>();

        services.AddScoped<IQueryHandler<GetCredentialByIdQuery, Result<CredentialDetailDto>>,
            GetCredentialByIdQueryHandler>();

        return services;
    }
}
