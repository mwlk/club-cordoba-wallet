
using ClubCordobaWallet.Application.Features.Credentials.Dtos;

namespace ClubCordobaWallet.Application.Features.Credentials.Queries.SearchMemberByDni;

// No existe en el enunciado -> es UX nuestra para autocompletar el form,
// buscando por prefijo de DNI (el operador puede no recordarlo completo).
// Una lista vacía es un resultado válido (socio nuevo), no un error -> por
// eso Result siempre en éxito, nunca Fail.
public class SearchMemberByDniQueryHandler(IMemberRepository memberRepository)
    : IQueryHandler<SearchMemberByDniQuery, Result<List<MemberSearchDto>>>
{
    private const int MaxResults = 10;

    public async Task<Result<List<MemberSearchDto>>> Handle(SearchMemberByDniQuery query, CancellationToken ct)
    {
        var members = await memberRepository.SearchByDniPrefixAsync(query.Dni, MaxResults, ct);

        var dtos = members
            .Select(m => new MemberSearchDto(m.Dni, m.FirstName, m.LastName, m.MemberNumber))
            .ToList();

        return Result<List<MemberSearchDto>>.Ok(dtos, "MembersSearched");
    }
}
