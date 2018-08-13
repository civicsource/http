using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Archon.AspNetCore
{
	/// <summary>
	/// When a query string parameter keyed by <c>accept</c> is provided in a request, this middleware
	/// overrides the HTTP <c>Accept</c> header with the value of the first matching parameter.
	/// </summary>
	public class AcceptHeaderMiddleware
	{
		readonly RequestDelegate next;

		public AcceptHeaderMiddleware(RequestDelegate next)
		{
			this.next = next;
		}

		public async Task Invoke(HttpContext context)
		{
			ManipulateHeaders(context.Request);
			await next(context);
		}

		void ManipulateHeaders(HttpRequest request)
		{
			string accept = request.Query["accept"];

			if (String.IsNullOrWhiteSpace(accept))
				return;

			request.Headers.Remove("Accept");
			request.Headers.Add("Accept", accept);
		}
	}
}