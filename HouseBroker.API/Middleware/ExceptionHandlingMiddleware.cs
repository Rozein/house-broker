using System.Net;
using HouseBroker.Application.CommonDto;
using HouseBroker.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace HouseBroker.API.Middleware;
/// <summary>
/// Custom exception handler
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (Exception exception)
        {
            _logger.LogError(exception, "Exception occurred: {Message} {StackTrace}",exception.Message, exception.StackTrace);


            var exceptionDetails = GetExceptionDetails(exception);

            var problemDetails = new ProblemDetails
            {
                Status = exceptionDetails.Status,
                Detail = exceptionDetails.Detail,
            };
            
            var error = new Error((HttpStatusCode)exceptionDetails.Status, exceptionDetails.Detail);

            var result = Response.Failure(error);

            if (exceptionDetails.Errors is not null)
            {
                problemDetails.Extensions["errors"] = exceptionDetails.Errors;
            }

            context.Response.StatusCode = exceptionDetails.Status;

            await context.Response.WriteAsJsonAsync(result);
        }
    }

    private static ExceptionDetails GetExceptionDetails(Exception exception)
    {
        return exception switch
        {
            ValidationException validationException => new ExceptionDetails(
                StatusCodes.Status400BadRequest,
                "Validation error",
                string.Join(", ", validationException.Errors.Select(error => new { error.PropertyName, error.ErrorMessage })),
                validationException.Errors),
            UnitOfWorkExceptions unitOfWorkExceptions => new ExceptionDetails(
                StatusCodes.Status500InternalServerError,
                "Transaction error",
                exception.Message,
                null
            ),
            NotFoundException notFoundException => new ExceptionDetails(
                StatusCodes.Status404NotFound,
                "NotFound error",
                exception.Message,
                null),
            _ => new ExceptionDetails(
                StatusCodes.Status500InternalServerError,
                "Server error",
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Production"?$"An unexpected error has occurred: {exception.Message}: {exception.StackTrace}":$"An unexpected error has occurred : {exception.Message}",
                null)
        };
    }

    private record ExceptionDetails(
        int Status,
        string Title,
        string Detail,
        IEnumerable<object>? Errors);
}