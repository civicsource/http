using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Archon.WebApi
{
	public class AuthorizeCorrectlyAttribute : AuthorizeAttribute
	{
		protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
		{
			var user = actionContext.ControllerContext.RequestContext.Principal;

			if (user == null || !user.Identity.IsAuthenticated)
			{
				actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Please authenticate to access this resource.");
			}
			else
			{
				actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden, "User does not have appropriate permissions to access this resource.");
			}
		}
	}
}