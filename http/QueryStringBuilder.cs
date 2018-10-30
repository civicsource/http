using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archon.Http
{
	public class QueryStringBuilder
	{
		readonly string baseUri;
		readonly Dictionary<string, List<string>> parameters = new Dictionary<string, List<string>>();

		public bool HasAny => parameters.Any();

		public QueryStringBuilder()
			: this(null) { }

		public QueryStringBuilder(string baseUri)
		{
			this.baseUri = ParseQueryString(baseUri);
		}

		string ParseQueryString(string uri)
		{
			if (String.IsNullOrWhiteSpace(uri))
				return String.Empty;

			int idx = uri.IndexOf("?");
			if (idx < 0)
				return uri;

			string uriWithoutQuery = uri.Substring(0, idx);
			string qs = uri.Substring(idx + 1, uri.Length - idx - 1);

			string[] parts = qs.Split('&');

			foreach (string part in parts)
			{
				if (part.Contains("="))
				{
					string[] kv = part.Split('=');
					Append(kv[0], kv[1]);
				}
				else
				{
					Append(part, "");
				}
			}

			return uriWithoutQuery;
		}

		public QueryStringBuilder Set(string key) => Set(key, "");

		public QueryStringBuilder Set(string key, string value)
		{
			parameters[key] = new List<string>() { value };
			return this;
		}

		public QueryStringBuilder Append(string key) => Append(key, "");

		public QueryStringBuilder Append(string key, string value)
		{
			if (!parameters.ContainsKey(key))
				parameters.Add(key, new List<string>());

			parameters[key].Add(value);
			return this;
		}

		public QueryStringBuilder Remove(string key)
		{
			parameters.Remove(key);
			return this;
		}

		public override string ToString()
		{
			if (!HasAny) return baseUri;

			var q = new StringBuilder($"{baseUri}?");

			foreach (string key in parameters.Keys)
			{
				foreach (string value in parameters[key])
				{
					if (!String.IsNullOrWhiteSpace(value))
					{
						q.Append($"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value)}&");
					}
					else
					{
						q.Append($"{Uri.EscapeDataString(key)}&");
					}
				}
			}

			q.Remove(q.Length - 1, 1);
			return q.ToString();
		}
	}
}