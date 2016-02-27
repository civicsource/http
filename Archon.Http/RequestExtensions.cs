using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;

namespace Archon.Http
{
	public static class RequestExtensions
	{
		public static HttpRequestMessage WithJsonContent<T>(this HttpRequestMessage request, T content)
		{
			request.Content = new ObjectContent<T>(content, new JsonMediaTypeFormatter());
			return request;
		}
	}
}