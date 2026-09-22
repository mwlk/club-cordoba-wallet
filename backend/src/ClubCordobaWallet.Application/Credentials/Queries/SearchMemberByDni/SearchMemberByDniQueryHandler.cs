using ClubCordobaWallet.Application.Credentials.Dtos;

namespace ClubCordobaWallet.Application.Credentials.Queries.SearchMemberByDni;

// No existe en el enunciado -> es UX nuestra para autocompletar el form.
// No es un caso de error real cuando no encuentra, por eso Result en vez
// de un 404: el frontend no debe tratar "socio nuevo" como un fallo.
public class SearchMemberByDniQueryHandler(IMemberRepository memberRepository)
    : IQueryHandler<SearchMemberByDniQuery, Result<MemberSearchDto>>
{
    public async Task<Result<MemberSearchDto>> Handle(SearchMemberByDniQuery query, CancellationToken ct)
    {
        var member = await memberRepository.GetByDniAsync(query.Dni, ct);
        if (member is null)
            return Result<MemberSearchDto>.Fail("MemberNotFound");

        return Result<MemberSearchDto>.Ok(
            new MemberSearchDto(member.FirstName, member.LastName, member.MemberNumber),
            "MemberFound");
    }
}
