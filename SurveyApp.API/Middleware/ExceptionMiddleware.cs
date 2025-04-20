using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;
using SurveyApp.API.Models;

namespace SurveyApp.API.Middleware
{
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
      catch (Exception ex)
      {
        _logger.LogError($"Something went wrong: {ex}");

        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";

        var errorResponse = new ErrorResponse
        {
          Message = "An unexpected error occurred.",
          Detail = ex.Message
        };

        await context.Response.WriteAsJsonAsync(errorResponse);
      }
    }
  }

  public class ErrorResponse
  {
    public string Message { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
  }
}
