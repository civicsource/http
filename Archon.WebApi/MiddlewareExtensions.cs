using Microsoft.AspNet.Builder;

namespace Archon.WebApi
{
	public static class MiddlewareExtensions
	{
		public static IApplicationBuilder UseEnhancements(this IApplicationBuilder app)
		{
			return app
				.UseMiddleware<AuthHeaderMiddleware>()
				.UseMiddleware<AuthorizeCorrectlyMiddleware>()
				.UseMiddleware<AcceptHeaderMiddleware>();
		}
	}
}
