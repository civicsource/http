using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Archon.WebApi.Tests
{
	public class AuthorizationTests
	{
		[Fact]
		public void can_process_bearer_tokens()
		{
			var auth = Authorization.Bearer("my-token");
			Assert.NotNull(auth);
			Assert.Null(auth.Username);
			Assert.Null(auth.Password);
			Assert.Equal("my-token", auth.Token);

			var header = auth.AsHeader();
			Assert.Equal("Bearer", header.Scheme);
			Assert.Equal("my-token", header.Parameter);

			auth = Authorization.Parse(header);
			Assert.NotNull(auth);
			Assert.Null(auth.Username);
			Assert.Null(auth.Password);
			Assert.Equal("my-token", auth.Token);
		}

		[Fact]
		public void can_process_basic_authentication()
		{
			var auth = Authorization.Basic("homer.simpson", "password");
			Assert.NotNull(auth);
			Assert.Null(auth.Token);
			Assert.Equal("homer.simpson", auth.Username);
			Assert.Equal("password", auth.Password);

			var header = auth.AsHeader();
			Assert.Equal("Basic", header.Scheme);
			Assert.NotNull(header.Parameter);
			Assert.NotEqual("", header.Parameter);
			Assert.DoesNotContain("homer.simpson", header.Parameter);
			Assert.DoesNotContain("password", header.Parameter);

			auth = Authorization.Parse(header);
			Assert.NotNull(auth);
			Assert.Null(auth.Token);

			Assert.Equal("homer.simpson", auth.Username);
			Assert.Equal("password", auth.Password);
		}
	}
}