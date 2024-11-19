
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace BupaAustraliaAPI.Middlewares;
public class GlobalExceptionMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            ProblemDetails problem = new()
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Type = "Server error",
                Title = "Server Error",
                Detail = ex.Message
            };
            string json = JsonSerializer.Serialize(problem);
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(json);
        }
    }
}