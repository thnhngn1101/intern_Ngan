using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Common.Application.Models;
using Common.Application.Settings;
using Common.Controllers;
using Common.Services;
using Common.Application.Exceptions;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using DocumentFormat.OpenXml.Drawing;
public class ValidateCurrentUserFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public ValidateCurrentUserFilter(IServiceProvider serviceProvider, IHttpContextAccessor httpContextAccessor)
    {
        _serviceProvider = serviceProvider;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.Controller is ControllerBase controller)
        {
            
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (actionDescriptor != null)
            {
                switch (actionDescriptor.ActionName)
                {
                    case "Authenticate":
                    case "AuthenticateWithAzure":
                    case "TestSyncAzureAccount":
                    case "AuthenticateWithAzureAsync":
                    //case "AuthenticateWithAzure":
                        // Skip validation for these actions
                        await next();
                        break;
                    default:
                        //var userServiceType = typeof(AuthUserService<>).MakeGenericType(typeof(BaseAppSetting));
                        //var authService = _serviceProvider.GetRequiredService(userServiceType);

                        //// Use reflection to call the GetUser method
                        //var user = _httpContextAccessor.HttpContext?.Items["User"] as AuthenticatedUserModel;
                        //if (user == null)
                        //{
                        //    throw new NonAuthenticateException("No Authen");
                        //}
                        //// Assuming your controller has a property "_currentUser" to hold this information
                        //var currentUserProperty = controller.GetType().GetProperty("_currentUser");
                        //currentUserProperty?.SetValue(controller, user);
                        await next();
                        break;

                }
            }
           
        }

       
    }
}