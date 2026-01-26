using System.Net;
using System.Text.Json;

namespace SaasApi.Middlewares;

public class GlobalErrorHandlingMiddleware(RequestDelegate next, ILogger<GlobalErrorHandlingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<GlobalErrorHandlingMiddleware> _logger = logger;

    // Este método intercepta TODAS as requisições
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Tenta passar a requisição para frente (pro Controller)
            await _next(context);
        }
        catch (Exception ex)
        {
            // Se der erro em qualquer lugar, cai aqui
            _logger.LogError(ex, "Ocorreu um erro não tratado na API.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = new
        {
            status = context.Response.StatusCode,
            message = "Ocorreu um erro interno no servidor.",
            detailed_error = exception.Message // Em produção, pode esconder isso aqui
        };

        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        return context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
    }
}