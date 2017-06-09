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

		[Fact]
		public void can_send_a_link_with_multiple_actions_and_parse_the_response ()
		{
			handler.Action = (req, c) => Task.FromResult(new HttpResponseMessage
			{
				Content = new StringContent("this is a test")
			});

			handler.Action = (req, c) => Task.FromResult(new HttpResponseMessage
			{
				Content = new StringContent("this is a test too")
			});

			string response = client.Send(new LinkWithResponse());
			Assert.Equal("this is a test", response);
		}

		[Fact]
		public void can_send_a_link_with_multiple_actions_and_handle_them_conditionally()
		{
			handler.Action = (req, c) =>
			{
				var uriString = req.RequestUri.AbsolutePath.ToString();
				if (uriString.Contains("OogaBooga"))
				{
					return Task.FromResult(new HttpResponseMessage { Content = new StringContent("this is a test") });
				}

				return null;
			};

			handler.Action = (req, c) => Task.FromResult(new HttpResponseMessage
			{
				Content = new StringContent("this is a test too")
			});

			string response = client.Send(new CustomLinkWithResponse("OogaBooga"));
			Assert.Equal("this is a test", response);
		}
	}
}