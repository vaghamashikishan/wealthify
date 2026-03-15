using System;
using wealthify.Exceptions;
using wealthify.Models;

namespace wealthify.Middlewares;

public class GlobalExceptionHandlerMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (System.Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, errMessage) = ex is CustomException customException
            ? (customException.StatusCode, customException.Message)
            : (StatusCodes.Status500InternalServerError, "An unexpected error occurred");

        context.Response.StatusCode = statusCode;

        var apiResponse = new ApiResponse
        {
            Success = false,
            Errors = [errMessage]
        };

        await context.Response.WriteAsJsonAsync(apiResponse);
    }
}
