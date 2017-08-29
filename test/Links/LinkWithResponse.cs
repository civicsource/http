using System.Net.Http;
using System.Threading.Tasks;

namespace Archon.Http.Tests.Links
{
	class LinkWithResponse : PlainLink, Link<string>
	{
		public async Task<string> ParseResponseAsync(HttpResponseMessage response)
		{
			return await response.Content.ReadAsStringAsync();
		}
	}
}