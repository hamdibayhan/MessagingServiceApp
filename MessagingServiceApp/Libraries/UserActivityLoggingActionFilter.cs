using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using MessagingServiceApp.Dto.ApiParameter;
using Microsoft.AspNetCore.Mvc.Abstractions;

namespace MessagingServiceApp.Libraries
{
    public class UserActivityLoggingActionFilter : ActionFilterAttribute
    {
        private readonly ILogger _logger;

        public UserActivityLoggingActionFilter(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger("UserActivityLoggingActionFilter");
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;
            var isUserAuthenticated = httpContext.User.Identity.IsAuthenticated;

            if(isUserAuthenticated)
            {
                SetAuthenticatedUserProperty(httpContext);
                SetRequestParamsProperty(context);
            }
            else
            {
                SetUnauthenticatedUserProperty(context.ActionArguments, context.ActionDescriptor);
            }

            _logger.LogInformation("UserAction Request ...");
            base.OnActionExecuting(context);
        }
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            SetResultObject(context);

            _logger.LogInformation("UserAction Result ...");
            base.OnActionExecuted(context);
        }

        private void SetAuthenticatedUserProperty(HttpContext httpContext)
        {
            var httpUser = httpContext.User;

            var userId = httpUser.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = httpUser.FindFirstValue(ClaimTypes.Name);
            var userEmail = httpUser.FindFirstValue(ClaimTypes.Email);

            var logUser = new LogUser()
            {
                Id = userId,
                UserName = userName,
                Email = userEmail
            };

            LogContext.PushProperty("User", JsonSerializer.Serialize(logUser));
        }

        private void SetUnauthenticatedUserProperty(IDictionary<string, object> parameters, ActionDescriptor actionName)
        {
            var logUser = new LogUser();
            actionName.RouteValues.TryGetValue("action", out string actionNameString);

            if (actionNameString == "Login")
            {
                var model = (LoginParams)parameters["model"];
                logUser.Email = model.Email;

                LogContext.PushProperty("User", JsonSerializer.Serialize(logUser));
            }
            else
            {
                var model  = (RegisterParams)parameters["model"];
                logUser.Email = model.Email;
                logUser.UserName = model.UserName;

                LogContext.PushProperty("User", JsonSerializer.Serialize(logUser));
            }
        }

        private void SetRequestParamsProperty(ActionExecutingContext context)
        {
            var requestParams = context.ActionArguments.Values;
            LogContext.PushProperty("RequestParams", JsonSerializer.Serialize(requestParams));
        }

        private void SetResultObject(ActionExecutedContext context)
        {
            var resultObject = context.Result;
            var objResult = resultObject as ObjectResult;

            LogContext.PushProperty("ResultObject", JsonSerializer.Serialize(objResult.Value));
        }
    }

    public class LogUser
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}
