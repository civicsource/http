using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;

namespace Archon.WebApi
{
	/// <summary>
	/// If it finds an auth token in the querystring, it moves it to the authorization header.
	/// </summary>
	public class AuthHeaderMiddleware
	{
		readonly RequestDelegate next;

		public AuthHeaderMiddleware(RequestDelegate next)
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
			bool hasAuthHeader = request.Headers.ContainsKey("Authorization");

			if (hasAuthHeader)
				return;

			string auth = request.Query["auth"];
			if (String.IsNullOrWhiteSpace(auth))
				return;

			request.Headers.Add("Authorization", "Bearer " + auth);
		}
	}
}