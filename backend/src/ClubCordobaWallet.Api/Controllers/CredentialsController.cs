using ClubCordobaWallet.Application.Features.Credentials.Commands.CreateCredential;
using ClubCordobaWallet.Application.Features.Credentials.Dtos;
using ClubCordobaWallet.Application.Features.Credentials.Queries.GetCredentialById;
using ClubCordobaWallet.Application.Features.Credentials.Queries.GetCredentials;
using ClubCordobaWallet.Application.Features.Credentials.Queries.SearchMemberByDni;
using ClubCordobaWallet.Application.Features.Credentials.Queries.GetActiveCredentialByDni;
using System.Resources;
using ClubCordobaWallet.Domain.Enums;

namespace ClubCordobaWallet.Api.Controllers;

// Único controller, tal cual exige el enunciado (4.2.1): "un único
// controller (...) que recibe el alta, arma el credentialSubject, invoca
// al Issuer, persiste la credencial y expone el listado". El buscador de
// socio (members/search) es una acción más de este mismo controller, no
// un controller nuevo -> ver docs/decisiones.md.
[ApiController]
[Route("api/credentials")]
public class CredentialsController(
    ICommandHandler<CreateCredentialCommand, Result<CreateCredentialResult>> createHandler,
    IQueryHandler<SearchMemberByDniQuery, Result<List<MemberSearchDto>>> searchHandler,
    IQueryHandler<GetCredentialsQuery, List<CredentialListDto>> listHandler,
    IQueryHandler<GetCredentialByIdQuery, Result<CredentialDetailDto>> detailHandler,
    IQueryHandler<GetActiveCredentialByDniQuery, Result<ActiveCredentialDto?>> activeCredentialHandler
) : ControllerBase
{
    // GET /api/credentials/members/active-credential?dni=301... (renovacion-credencial-activa)
    // Consulta previa del frontend para mostrar el popup de confirmación
    // con la fecha real antes de intentar el alta. Siempre 200, data:null
    // si no hay socio o no tiene credencial vigente -> no es un error.
    [HttpGet("members/active-credential")]
    public async Task<IActionResult> GetActiveCredential([FromQuery] string dni, CancellationToken ct)
    {
        var result = await activeCredentialHandler.Handle(new GetActiveCredentialByDniQuery(dni), ct);
        return Ok(ToResponse(result));
    }

    // GET /api/credentials/members/search?dni=301 (prefijo, no exacto)
    // Autocompletado de UX para el form de alta: devuelve hasta 10 socios
    // cuyo DNI empieza con el prefijo dado. Siempre responde 200 con una
    // lista (posiblemente vacía) -> "sin coincidencias" es un resultado
    // válido (socio nuevo), no un error.
    [HttpGet("members/search")]
    public async Task<IActionResult> SearchMember([FromQuery] string dni, CancellationToken ct)
    {
        var result = await searchHandler.Handle(new SearchMemberByDniQuery(dni), ct);
        return Ok(ToResponse(result));
    }

    // GET /api/credentials
    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        var list = await listHandler.Handle(new GetCredentialsQuery(), ct);
        return Ok(list);
    }

    // GET /api/credentials/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await detailHandler.Handle(new GetCredentialByIdQuery(id), ct);
        if (!result.Success) return NotFound(ToResponse(result));
        return Ok(ToResponse(result));
    }

    // POST /api/credentials
    // Body: { nombre, apellido, dni, categoria, foto } — tal cual 4.1.3.
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCredentialRequest request, CancellationToken ct)
    {
        var command = new CreateCredentialCommand(request.Nombre, request.Apellido, request.Dni, request.Categoria, request.Foto, request.ConfirmarRenovacion);
        var result = await createHandler.Handle(command, ct);
        if (!result.Success) return BadRequest(ToResponse(result));
        return StatusCode(201, ToResponse(result));
    }

    // Resx: Messages.resx / Errors.resx en Infrastructure/Resources.
    // Se accede vía ResourceManager directo (sin clases Designer autogeneradas)
    // para evitar depender del generador de VS en este scaffold inicial.
    private static readonly ResourceManager MessagesRm =
        new("ClubCordobaWallet.Infrastructure.Resources.Messages", typeof(ClubCordobaWallet.Infrastructure.Services.TenantService).Assembly);
    private static readonly ResourceManager ErrorsRm =
        new("ClubCordobaWallet.Infrastructure.Resources.Errors", typeof(ClubCordobaWallet.Infrastructure.Services.TenantService).Assembly);

    private static object ToResponse<T>(Result<T> result) => new
    {
        success = result.Success,
        message = result.Success
            ? (result.MessageKey is not null ? MessagesRm.GetString(result.MessageKey) : null)
            : ErrorsRm.GetString(result.ErrorKey!),
        data = result.Data
    };
}

public record CreateCredentialRequest(string Nombre, string Apellido, string Dni, MemberCategory Categoria, string Foto, bool ConfirmarRenovacion = false);
