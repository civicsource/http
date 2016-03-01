using Microsoft.AspNet.Builder;

namespace Archon.Http
{
	public static class AcceptHeaderAppBuilderExtensions
	{
		public static IApplicationBuilder UseAcceptHeaderRewriter(this IApplicationBuilder app)
		{
			return app.UseMiddleware<AcceptHeaderMiddleware>();
		}
	}
}
