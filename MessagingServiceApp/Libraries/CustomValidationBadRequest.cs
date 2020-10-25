using System.Collections.Generic;
using MessagingServiceApp.Dto.ApiResponse;
using Microsoft.AspNetCore.Mvc;

public class CustomValidationBadRequest : ValidationProblemDetails
{
    public CustomValidationBadRequest(ActionContext context)
    {
        ConstructErrorMessages(context);
        Response<IDictionary<string, string[]>>.GetError(null, null, Errors);
    }

    private void ConstructErrorMessages(ActionContext context)
    {
        foreach (var keyModelStatePair in context.ModelState)
        {
            var key = keyModelStatePair.Key;
            var errors = keyModelStatePair.Value.Errors;
            if (errors != null && errors.Count > 0)
            {
                var errorMessages = new string[errors.Count];
                for (var i = 0; i < errors.Count; i++)
                {
                    errorMessages[i] = errors[i].ErrorMessage;
                }

                Errors.Add(key, errorMessages);
            }
        }
    }
}