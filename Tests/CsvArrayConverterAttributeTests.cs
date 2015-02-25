using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using Xunit;

namespace Archon.WebApi.Tests
{
	public class CsvArrayConverterAttributeTests
	{
		protected CsvArrayConverterAttribute filter;
		protected Type type;

		HttpActionContext SetupContext(string uri)
		{
			filter = new CsvArrayConverterAttribute("ids", type);
			return ContextUtil.CreateActionContext(controllerContext: ContextUtil.CreateControllerContext(request: new HttpRequestMessage(HttpMethod.Post, uri)));
		}

		[Fact]
		public void can_convert_list_of_strings()
		{
			type = typeof(string);
			var ctx = SetupContext("http://localhost/api/controller/action?ids=45,86,12");
			ctx.ActionArguments.Add("ids", "");

			filter.OnActionExecuting(ctx);

			var ids = ctx.ActionArguments["ids"];

			Assert.NotNull(ids);
			var values = (object[])ids;
			Assert.Equal("45", values[0]);
		}

		[Fact]
		public void can_handle_emtpy_args()
		{
			type = typeof(string);
			var ctx = SetupContext("http://localhost/api/controller/action");
			ctx.ActionArguments.Add("ids", "");

			filter.OnActionExecuting(ctx);

			var ids = ctx.ActionArguments["ids"];

			Assert.NotNull(ids);
			var values = (object[])ids;
			Assert.Empty(values);
		}

		[Fact]
		public void can_convert_list_of_guids()
		{
			type = typeof(Guid);
			var ctx = SetupContext("http://localhost/api/controller/action?ids=BFD50AD0-9B01-4965-96AC-A54E6550EF51,9912B9CB-C8B0-4EA4-A3D2-E89FC0BF549D");
			ctx.ActionArguments.Add("ids", "");

			filter.OnActionExecuting(ctx);

			var ids = ctx.ActionArguments["ids"];

			Assert.NotNull(ids);
			var values = (object[])ids;
			Assert.Equal(new Guid("BFD50AD0-9B01-4965-96AC-A54E6550EF51"), values[0]);
		}

		[Fact]
		public void can_convert_list_of_integers()
		{
			type = typeof(int);
			var ctx = SetupContext("http://localhost/api/controller/action?ids=1,2,3,4,5,6,7,8");
			ctx.ActionArguments.Add("ids", "");

			filter.OnActionExecuting(ctx);

			var ids = ctx.ActionArguments["ids"];

			Assert.NotNull(ids);
			var values = (object[])ids;
			Assert.Equal(3, values[2]);
		}
	}
}