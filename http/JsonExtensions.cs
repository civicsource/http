using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Archon.Http
{
	public static class JsonExtensions
	{
		public static async Task<T> ReadJsonAsync<T>(this HttpContent content)
		{
			string json = await content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<T>(json);
		}

		public static HttpRequestMessage WithJsonContent<T>(this HttpRequestMessage req, T obj)
		{
			return WithJsonContent<T>(req, obj, "application/json");
		}

		public static HttpRequestMessage WithJsonContent<T>(this HttpRequestMessage req, T obj, string contentType)
		{
			string json = JsonConvert.SerializeObject(obj);
			req.Content = new StringContent(json, Encoding.UTF8, contentType);
			return req;
		}
	}
}
