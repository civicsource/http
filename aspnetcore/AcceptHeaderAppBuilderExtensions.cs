using Microsoft.AspNetCore.Builder;

namespace Archon.AspNetCore
{
	public static class AppBuilderExtensions
	{
		/// <summary>
		/// When a query string parameter keyed by <c>accept</c> is provided in a request, this middleware
		/// overrides the HTTP <c>Accept</c> header with the value of the first matching parameter.
		/// </summary>
		public static IApplicationBuilder UseAcceptHeaderRewriter(this IApplicationBuilder app)
		{
			return app.UseMiddleware<AcceptHeaderMiddleware>();
		}

		/// <summary>
		/// When no <c>Authorization</c> header is provided in a request, this middleware captures the first
		/// query string parameter keyed by <c>auth</c>, and adds the parameter's value as a <c>Bearer</c> token.
		/// </summary>
		public static IApplicationBuilder UseAuthHeaderRewriter(this IApplicationBuilder app)
		{
			return app.UseMiddleware<AuthHeaderMiddleware>();
		}
	}
}
