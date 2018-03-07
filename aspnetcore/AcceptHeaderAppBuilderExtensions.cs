using Microsoft.AspNetCore.Builder;

namespace Archon.AspNetCore
{
	public static class AcceptHeaderAppBuilderExtensions
	{
		public static IApplicationBuilder UseAcceptHeaderRewriter(this IApplicationBuilder app)
		{
			return app.UseMiddleware<AcceptHeaderMiddleware>();
		}
	}
}
