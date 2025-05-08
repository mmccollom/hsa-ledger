using System.Text.Json;
using FluentValidation;
using HsaLedger.Shared.Wrapper;

namespace HsaLedger.Server.Middleware;

public class ValidationExceptionMiddleware
{
    private readonly RequestDelegate _next;


    public ValidationExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException validationException)
        {
            var errors = validationException.Errors.Select(validationFailure =>
                $"{validationFailure.PropertyName}: {validationFailure.ErrorMessage}").ToList();

            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = 200;

            var error = await Result<string?>.FailAsync(errors);
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,

            };
            var result = JsonSerializer.Serialize(error, options);
            await response.WriteAsync(result);
        }
        catch (Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = 200;

            var error = await Result<string?>.FailAsync(exception.ToString());
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,

            };
            var result = JsonSerializer.Serialize(error, options);
            await response.WriteAsync(result);
        }
    }
}