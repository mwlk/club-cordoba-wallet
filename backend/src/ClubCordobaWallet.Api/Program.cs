using ClubCordobaWallet.Api.Middleware;
using ClubCordobaWallet.Application;
using ClubCordobaWallet.Infrastructure;
using ClubCordobaWallet.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using System.Resources;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, cfg) => cfg
    .ReadFrom.Configuration(ctx.Configuration)
    .WriteTo.Console());

// Fail fast: sin clave HMAC la app no debe levantar (si falla la firma,
// el enunciado exige que no se persista nada; mejor detectarlo al boot).
var hmacKey = builder.Configuration["Issuer:HmacKey"];
if (string.IsNullOrWhiteSpace(hmacKey))
    throw new InvalidOperationException("Issuer:HmacKey no está configurado (env var / user-secrets).");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins(builder.Configuration["Cors:AllowedOrigins"] ?? "http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

// Aplica migrations automáticamente al arrancar (decisión: EF Core
// Migrations en vez de init.sql manual -> ver docs/decisiones.md).
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    // Documento OpenAPI generado por Microsoft.AspNetCore.OpenApi (nativo del
    // template desde .NET 9+) + Scalar como UI interactiva, reemplazo de
    // Swagger UI (ver docs/decisiones.md).
    app.MapOpenApi();
    app.MapScalarApiReference();
    // Raíz -> Scalar para que al levantar en dev caigas directo en la doc.
    // Redirect relativo: hereda http/https del request (no hay TLS configurado).
    app.MapGet("/", () => Results.Redirect("/scalar/v1"));
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

// 404 global con result pattern: las rutas que ninguna acción matchea
// (p. ej. /api/credentials/abc con la constraint {id:guid}) llegan acá
// con 404 y body vacío. Los 404 que el controller ya escribe con body
// (NotFound(ToResponse(result))) tienen HasStarted=true y se saltean.
app.UseStatusCodePages(async statusContext =>
{
    if (statusContext.HttpContext.Response.StatusCode != StatusCodes.Status404NotFound) return;

    var errorsRm = new ResourceManager(
        "ClubCordobaWallet.Infrastructure.Resources.Errors",
        typeof(ClubCordobaWallet.Infrastructure.Services.TenantService).Assembly);

    statusContext.HttpContext.Response.ContentType = "application/json";
    await statusContext.HttpContext.Response.WriteAsJsonAsync(new
    {
        success = false,
        message = errorsRm.GetString("ResourceNotFound"),
        data = (object?)null
    });
});

app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
