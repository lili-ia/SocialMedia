using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SocialMedia.Application.Contracts;

namespace SocialMedia.ActionFilters;

public class RequireUserIdNotNullFilter : IActionFilter
{
    private readonly IUserContext _userContext;

    public RequireUserIdNotNullFilter(IUserContext userContext)
    {
        _userContext = userContext;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (_userContext.UserId == null)
        {
            context.Result = new UnauthorizedResult();
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}
