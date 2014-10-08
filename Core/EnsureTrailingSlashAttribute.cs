using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Archon.WebApi
{
	public class EnsureTrailingSlashAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(HttpActionContext ctx)
		{
			Uri uri = ctx.Request.RequestUri;
			string uriString = uri.WithoutQuery();

			if (!uriString.EndsWith("/"))
			{
				ctx.Response = ctx.Request.CreateResponse(HttpStatusCode.MovedPermanently);
				ctx.Response.Headers.Location = new Uri(uriString + "/" + uri.Query);
			}
		}
	}
}