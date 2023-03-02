using System.Net;
using Microsoft.AspNetCore.Mvc.Filters;
using web.Infrastructure.ActionResults;
using web.Infrastructure.Exceptions;

namespace web.Infrastructure.Filters;

public class GlobalExceptionFilter:IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;
    private readonly IWebHostEnvironment _env;
    public GlobalExceptionFilter( ILogger<GlobalExceptionFilter> logger, IWebHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }
    
    public void OnException(ExceptionContext context)
    {
        _logger.LogError(new EventId(context.Exception.HResult), context.Exception, "{ExceptionMessage}", context.Exception.Message);
        if (context.Exception.GetType() == typeof(BasketDomainException))
        {
            var errorResponse = new ErrorResponse
            {
                Messages = new[] { context.Exception.Message }
            };
          

            context.Result = new InternalServerErrorObjectResult(errorResponse);
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
        else
        {
            var errorResponse = new ErrorResponse
            {
                Messages = new[] { "An error occurred. Try it again." }
            };
            
            if (_env.IsDevelopment())
            {
                errorResponse.DeveloperMessage = context.Exception;
            }
            context.Result = new InternalServerErrorObjectResult(errorResponse);
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }

        context.ExceptionHandled = true;
    }
}