using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace web.Infrastructure.Filters;

public class ValidateModelStateFilter:ActionFilterAttribute
{

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ModelState.IsValid)
        {
            return;
        }

        var validationErrors = context.ModelState.Keys.SelectMany(x => context.ModelState[x].Errors)
            .Select(e => e.ErrorMessage).ToArray();

        var errorResponse = new ErrorResponse
        {
            Messages = validationErrors
        };
        context.Result = new BadRequestObjectResult(errorResponse);
    }
}