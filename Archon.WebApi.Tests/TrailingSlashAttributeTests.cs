using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using Xunit;

namespace Archon.WebApi.Tests
{
	public class TrailingSlashAttributeTests
	{
		readonly EnsureTrailingSlashAttribute filter;

		public TrailingSlashAttributeTests()
		{
			filter = new EnsureTrailingSlashAttribute();
		}

		HttpActionContext SetupContext(string uri)
		{
			return ContextUtil.CreateActionContext(controllerContext: ContextUtil.CreateControllerContext(request: new HttpRequestMessage(HttpMethod.Get, uri)));
		}

		[Fact]
		public void can_issue_redirect_for_request_with_no_trailing_slash()
		{
			var ctx = SetupContext("http://localhost/api");

			filter.OnActionExecuting(ctx);

			Assert.NotNull(ctx.Response);
			Assert.Equal(HttpStatusCode.MovedPermanently, ctx.Response.StatusCode);
			Assert.Equal("http://localhost/api/", ctx.Response.Headers.Location.ToString());
		}

		[Fact]
		public void can_issue_redirect_for_request_with_no_trailing_slash_and_a_query_string()
		{
			var ctx = SetupContext("http://localhost/api?one=two&three=four");

			filter.OnActionExecuting(ctx);

			Assert.NotNull(ctx.Response);
			Assert.Equal(HttpStatusCode.MovedPermanently, ctx.Response.StatusCode);
			Assert.Equal("http://localhost/api/?one=two&three=four", ctx.Response.Headers.Location.ToString());
		}

		[Fact]
		public void can_not_issue_redirect_for_request_with_trailing_slash()
		{
			var ctx = SetupContext("http://localhost/api/");

			filter.OnActionExecuting(ctx);

			Assert.Null(ctx.Response);
		}

		[Fact]
		public void can_not_issue_redirect_for_request_with_trailing_slash_and_a_query_string()
		{
			var ctx = SetupContext("http://localhost/api/?one=two&three=four");

			filter.OnActionExecuting(ctx);

			Assert.Null(ctx.Response);
		}
	}
}