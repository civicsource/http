using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Archon.WebApi
{
	public interface Link
	{
		HttpRequestMessage CreateRequest();
	}

	public interface Link<TResponse> : Link
	{
		Task<TResponse> ParseResponse(HttpResponseMessage response);
	}
}