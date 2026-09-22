using Serilog;

namespace ClubCordobaWallet.Api.Middleware;

// Middleware global de errores: los controllers no llevan try/catch.
// Cualquier excepción no controlada cae acá -> se loguea y se devuelve
// un response uniforme.
public class ExceptionHandlingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unhandled exception on {Path}", context.Request.Path);
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                message = "Ocurrió un error inesperado. Intente nuevamente.",
                data = (object?)null
            });
        }
    }
}
