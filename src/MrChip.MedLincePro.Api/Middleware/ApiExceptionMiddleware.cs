using System.Net;
using System.Text.Json;
using MrChip.MedLincePro.Api.ViewModels;

namespace MrChip.MedLincePro.Api.Middleware;

public sealed class ApiExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiExceptionMiddleware> _logger;

    public ApiExceptionMiddleware(RequestDelegate next, ILogger<ApiExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Erro de configuração/regra operacional em {Path}", context.Request.Path);
            await EscreverRespostaAsync(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado em {Path}", context.Request.Path);
            await EscreverRespostaAsync(context, HttpStatusCode.InternalServerError, "Erro interno ao processar a requisição.");
        }
    }

    private static async Task EscreverRespostaAsync(HttpContext context, HttpStatusCode statusCode, string mensagem)
    {
        if (context.Response.HasStarted) return;

        context.Response.Clear();
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var payload = ApiResponse<object>.Falha(mensagem);
        await context.Response.WriteAsync(JsonSerializer.Serialize(payload, new JsonSerializerOptions(JsonSerializerDefaults.Web)));
    }
}
