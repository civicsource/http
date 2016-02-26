using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Archon.WebApi.Tests.Links;
using Xunit;

namespace Archon.WebApi.Tests
{
	public class LinkTests
	{
		readonly HttpClient client;
		readonly FakeHttpHandler handler;

		public LinkTests()
		{
			client = new HttpClient(handler = new FakeHttpHandler());
		}

		[Fact]
		public void can_send_a_plain_link()
		{
			var response = client.Send(new PlainLink());
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		}

		[Fact]
		public void can_send_a_link_with_a_response_and_parse_the_response()
		{
			handler.Action = (req, c) => Task.FromResult(new HttpResponseMessage
			{
				Content = new StringContent("this is a test")
			});

			string response = client.Send(new LinkWithResponse());
			Assert.Equal("this is a test", response);
		}
	}
}