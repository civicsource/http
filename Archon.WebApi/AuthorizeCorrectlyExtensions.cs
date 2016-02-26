using Microsoft.AspNet.Builder;

namespace Archon.WebApi
{
	public static class AuthorizeCorrectlyExtensions
	{
		public static IApplicationBuilder UseCorrectAuthorization(this IApplicationBuilder app)
		{
			return app.UseMiddleware<AuthorizeCorrectlyMiddleware>();
		}
	}
}
