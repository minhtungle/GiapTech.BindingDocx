using System.Net;
using System.Text.Json;
using GiapTech.BindingDocx.Application.Common.Exceptions;
using GiapTech.BindingDocx.Application.Common.Models;

namespace GiapTech.BindingDocx.Api.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, response) = exception switch
        {
            ValidationException ve => (
                HttpStatusCode.BadRequest,
                ApiResponse<object>.Fail("Validation failed.", ve.Errors)),

            NotFoundException nfe => (
                HttpStatusCode.NotFound,
                ApiResponse<object>.Fail(nfe.Message)),

            UnauthorizedException ue => (
                HttpStatusCode.Unauthorized,
                ApiResponse<object>.Fail(ue.Message)),

            InsufficientTokensException ite => (
                HttpStatusCode.PaymentRequired,
                ApiResponse<object>.Fail(ite.Message)),

            _ => (
                HttpStatusCode.InternalServerError,
                ApiResponse<object>.Fail("An internal server error occurred."))
        };

        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }
}
