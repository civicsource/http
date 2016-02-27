using System.Net.Http;

namespace Archon.Http.Tests.Links
{
	class PlainLink : Link
	{
		public HttpRequestMessage CreateRequest()
		{
			return new HttpRequestMessage(HttpMethod.Get, "http://example.com/");
		}
	}
}