using System;
using System.Net;
using System.Net.Http;

namespace Archon.WebApi
{
	public static class RequestExtensions
	{
		public static HttpResponseMessage CreateNotFoundResponse(this HttpRequestMessage request, string name, Guid id)
		{
			return request.CreateErrorResponse(HttpStatusCode.NotFound, BuildNotFoundMessage(name, id.ToString()));
		}

		public static HttpResponseMessage CreateNotFoundResponse(this HttpRequestMessage request, string name, string identifier)
		{
			return request.CreateErrorResponse(HttpStatusCode.NotFound, BuildNotFoundMessage(name, identifier));
		}

		static string BuildNotFoundMessage(string name, string identifier)
		{
			return String.Format("Could not find {0} with identifier '{1}'.", name.ToLower(), identifier);
		}
	}
}