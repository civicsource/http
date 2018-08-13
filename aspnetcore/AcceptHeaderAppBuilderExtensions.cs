using Microsoft.AspNetCore.Builder;

namespace Archon.AspNetCore
{
	public static class AppBuilderExtensions
	{
		public static IApplicationBuilder UseAcceptHeaderRewriter(this IApplicationBuilder app)
		{
			return app.UseMiddleware<AcceptHeaderMiddleware>();
		}

		public static IApplicationBuilder UseAuthHeaderRewriter(this IApplicationBuilder app)
		{
			return app.UseMiddleware<AuthHeaderMiddleware>();
		}
	}
}
