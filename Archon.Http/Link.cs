using System.Net.Http;
using System.Threading.Tasks;

namespace Archon.Http
{
	public interface Link
	{
		HttpRequestMessage CreateRequest();
	}

	public interface Link<TResponse> : Link
	{
		Task<TResponse> ParseResponseAsync(HttpResponseMessage response);
	}
}