using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace Archon.WebApi
{
	/// <summary>
	/// Converts common domain-level exceptions into appropriate HTTP responses.
	/// </summary>
	public class DomainExceptionFilterAttribute : ExceptionFilterAttribute
	{
		public override void OnException(HttpActionExecutedContext context)
		{
			if (context.Exception is ArgumentException)
				context.Response = context.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid Argument", context.Exception);

			if (context.Exception is InvalidOperationException)
				context.Response = context.Request.CreateErrorResponse(HttpStatusCode.Conflict, "Invalid Operation", context.Exception);
		}
	}
}