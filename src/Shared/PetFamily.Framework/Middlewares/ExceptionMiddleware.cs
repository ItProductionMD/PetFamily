using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Exceptions;
using PetFamily.SharedKernel.Errors;

namespace PetFamily.Framework.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
        catch (UserNotAuthenticatedException ex)
        {
            _logger.LogError(ex, message: "UserNotAuthenticatedException:{Message}", ex.Message);

            var error = Error.Authorization(ex.Message);

            var envelope = Envelope.Failure(error);

            context.Response.ContentType = "application/json";

            context.Response.StatusCode = (int)StatusCodes.Status401Unauthorized;

            await context.Response.WriteAsJsonAsync(envelope);
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, message: "Exception:{Exception}", ex.Message);

            var envelope = Envelope.Failure(ex.Error);

            context.Response.ContentType = "application/json";

            context.Response.StatusCode = (int)StatusCodes.Status400BadRequest;

            await context.Response.WriteAsJsonAsync(envelope);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}-ExceptionMiddleware caught an exception");

            _logger.LogError(ex, message: "Exception:{Exception}", ex.Message);

            var error = Error.InternalServerError("Unexpected error!");

            var envelope = Envelope.Failure(error);

            context.Response.ContentType = "application/json";

            context.Response.StatusCode = (int)StatusCodes.Status500InternalServerError;

            await context.Response.WriteAsJsonAsync(envelope);
        }
    }
}
