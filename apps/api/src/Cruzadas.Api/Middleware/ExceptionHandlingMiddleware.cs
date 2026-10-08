using System.Net;
using Cruzadas.Application.Interfaces;
using Cruzadas.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Cruzadas.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IAppLogger? _appLogger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IAppLogger? appLogger = null)
    {
        _next = next;
        _logger = logger;
        _appLogger = appLogger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (status, title, detail) = exception switch
        {
            KeyNotFoundException knf => (HttpStatusCode.NotFound, "Recurso não encontrado", knf.Message),
            QuizNotPublishedException qnp => (HttpStatusCode.BadRequest, "Quiz indisponível", qnp.Message),
            AttemptAlreadyCompletedException aac => (HttpStatusCode.Conflict, "Tentativa já concluída", aac.Message),
            InvalidAttemptAnswerException iaa => (HttpStatusCode.UnprocessableEntity, "Resposta inválida", iaa.Message),
            ArgumentException ae => (HttpStatusCode.BadRequest, "Parâmetro inválido", ae.Message),
            BadHttpRequestException bhre => (HttpStatusCode.BadRequest, "Requisição inválida", bhre.Message),
            _ => (HttpStatusCode.InternalServerError, "Erro interno do servidor", "Ocorreu um erro inesperado ao processar a requisição.")
        };

        if (status == HttpStatusCode.InternalServerError)
        {
            _logger.LogError(exception, "Erro inesperado processando requisição {Path}", context.Request.Path);

            _appLogger?.LogError(
                category: "Cruzadas.Api.Middleware",
                message: $"Erro inesperado ao processar requisição {context.Request.Method} {context.Request.Path}: {exception.Message}",
                exception: exception,
                eventName: "UnhandledException",
                properties: new
                {
                    Path = context.Request.Path.Value,
                    Method = context.Request.Method,
                    StatusCode = (int)status,
                    TraceIdentifier = context.TraceIdentifier
                });
        }
        else
        {
            _logger.LogWarning("Falha de negócio ao processar requisição {Path}: {Message}", context.Request.Path, exception.Message);

            _appLogger?.LogWarning(
                category: "Cruzadas.Api.Middleware",
                message: $"Falha ao processar requisição {context.Request.Method} {context.Request.Path}: {exception.Message}",
                eventName: "HttpRequestFailed",
                properties: new
                {
                    Path = context.Request.Path.Value,
                    Method = context.Request.Method,
                    StatusCode = (int)status,
                    ErrorType = exception.GetType().Name
                });
        }

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)status;

        var problemDetails = new ProblemDetails
        {
            Status = (int)status,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}
