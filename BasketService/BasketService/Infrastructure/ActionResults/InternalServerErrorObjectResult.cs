using Microsoft.AspNetCore.Mvc;

namespace web.Infrastructure.ActionResults;

public class InternalServerErrorObjectResult:ObjectResult
{
    public InternalServerErrorObjectResult(object? value) : base(value)
    {
        StatusCode = StatusCodes.Status500InternalServerError;
    }
}