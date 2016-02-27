using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Internal;
using Xunit;

namespace Archon.Http.Tests
{
	public class AuthHeaderMiddlewareTests
	{
		readonly HttpContext context;
		readonly AuthHeaderMiddleware middleware;

		public AuthHeaderMiddlewareTests()
		{
			context = new DefaultHttpContext();
			middleware = new AuthHeaderMiddleware(innerContext => Task.FromResult(0));
		}

		[Fact]
		public async Task can_parse_auth_parameter_to_bearer_token_header()
		{
			context.Request.QueryString = new QueryString("?auth=my-token");

			await middleware.Invoke(context);

			Assert.True(context.Request.Headers.ContainsKey("Authorization"));
			Assert.Equal("Bearer my-token", context.Request.Headers["Authorization"]);
		}

		[Fact]
		public async Task can_not_parse_other_parameters_as_bearer_token_header()
		{
			context.Request.QueryString = new QueryString("?auths=my-token");

			await middleware.Invoke(context);

			Assert.False(context.Request.Headers.ContainsKey("Authorization"));
		}
	}
}