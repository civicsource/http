using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Archon.AspNetCore
{
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