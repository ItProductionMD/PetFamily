using PetFamily.API.Responce;
using PetFamily.Domain.DomainError;
using System.Diagnostics;

namespace PetFamily.API.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next ,ILogger<ExceptionMiddleware> logger)
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
        catch (Exception ex)
        {
            Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}-ExceptionMiddleware caught an exception");
           
            _logger.LogError(ex,message:"Exception:{Exception}",ex.Message);

            var error = Error.InternalServerError("Unexpected error!");

            var envelope = Envelope.Failure(error);

            context.Response.ContentType = "application/json";

            context.Response.StatusCode = (int)StatusCodes.Status500InternalServerError;

            await context.Response.WriteAsJsonAsync(envelope);
         
        }
    }
}
