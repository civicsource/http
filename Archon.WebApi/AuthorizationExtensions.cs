using Microsoft.AspNet.Builder;

namespace Archon.WebApi
{
	public static class AuthorizationExtensions
	{
		public static IApplicationBuilder UseAuthEnhancements(this IApplicationBuilder app)
		{
			return app
				.UseMiddleware<AuthHeaderMiddleware>()
				.UseMiddleware<AuthorizeCorrectlyMiddleware>();
		}
	}
}
