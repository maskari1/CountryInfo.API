namespace CountryInfo.API.Filters
{
    using System.Net;
    using System.Net.Http;
    using System.Web.Http.Filters;
    public class ValidateFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    actionContext.ModelState);
            }
            else
            {
                if (actionContext.ActionArguments.ContainsValue(null))
                {
                    actionContext.Response = actionContext.Request.CreateErrorResponse(
                        HttpStatusCode.BadRequest, "The argument cannot be null");
                }
                else
                {
                    base.OnActionExecuting(actionContext);
                }               
            }
        }
    }
}