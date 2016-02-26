using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Archon.WebApi
{
	/// <remarks>
	/// http://www.asp.net/web-api/overview/formats-and-model-binding/model-validation-in-aspnet-web-api
	/// </remarks>
	public class ValidateModelAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(HttpActionContext actionContext)
		{
			if (!actionContext.ModelState.IsValid)
			{
				actionContext.Response = actionContext.Request.CreateErrorResponse(
					HttpStatusCode.BadRequest, actionContext.ModelState);
			}
		}
	}
}