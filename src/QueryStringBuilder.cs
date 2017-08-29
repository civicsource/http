using System.Collections.Generic;
using System.Text;

namespace Archon.Http
{
	public class QueryStringBuilder
	{
		readonly List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>();

		public bool HasAny
		{
			get { return parameters.Count > 0; }
		}

		public QueryStringBuilder Append(string key, string value)
		{
			parameters.Add(new KeyValuePair<string, string>(key, value));
			return this;
		}

		public override string ToString()
		{
			if (!HasAny) return "";

			var q = new StringBuilder("?");

			foreach (var item in parameters)
			{
				q.Append($"{item.Key}={item.Value}&");
			}

			q.Remove(q.Length - 1, 1);
			return q.ToString();
		}
	}
}