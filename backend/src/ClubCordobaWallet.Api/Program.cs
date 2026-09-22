using ClubCordobaWallet.Api.Middleware;
using ClubCordobaWallet.Application;
using ClubCordobaWallet.Infrastructure;
using ClubCordobaWallet.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;

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
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
