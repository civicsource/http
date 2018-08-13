using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Archon.AspNetCore
{
	/// <summary>
	/// When no <c>Authorization</c> header is provided in a request, this middleware captures the first
	/// query string parameter keyed by <c>auth</c>, and adds the parameter's value as a <c>Bearer</c> token.
	/// </summary>
	public class AuthHeaderMiddleware
	{
		private readonly RequestDelegate next;

		public AuthHeaderMiddleware(RequestDelegate next)
		{
			this.next = next;
		}

		public async Task Invoke(HttpContext context)
		{
			ManipulateHeaders(context.Request);

			await next(context);
		}

		private static void ManipulateHeaders(HttpRequest request)
		{
			if (request.Headers["Authorization"].Any())
				return;

			string authQueryValue = request.Query["auth"];

			if (!string.IsNullOrEmpty(authQueryValue))
			{
				request.Headers["Authorization"] = new StringValues($"Bearer {authQueryValue}");
			}
		}
	}
}
