using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace Archon.Http
{
	public class Authorization
	{
		public string Token { get; private set; }

		public string Username { get; private set; }
		public string Password { get; private set; }

		private Authorization() { }

		public static Authorization Bearer(string token)
		{
			if (String.IsNullOrWhiteSpace(token))
				throw new ArgumentNullException("token");

			return new Authorization
			{
				Token = token
			};
		}

		public static Authorization Basic(string username, string password)
		{
			if (String.IsNullOrWhiteSpace(username))
				throw new ArgumentNullException("username");

			if (String.IsNullOrWhiteSpace(password))
				throw new ArgumentNullException("password");

			return new Authorization
			{
				Username = username,
				Password = password
			};
		}

		public static Authorization Parse(AuthenticationHeaderValue header)
		{
			if (header == null)
				return null;

			if (header.Scheme == "Bearer")
				return Authorization.Bearer(header.Parameter);

			if (header.Scheme == "Basic")
			{
				byte[] bytes = Convert.FromBase64String(header.Parameter);
				string decoded = Encoding.UTF8.GetString(bytes);

				string[] parts = decoded.Split(':');

				if (parts.Length < 2)
					return null;

				string username = parts[0];
				string password = parts.Skip(1).Aggregate((s, s2) => s + s2); //assume any colons are in the password, username has no colons

				if (String.IsNullOrWhiteSpace(username) || String.IsNullOrWhiteSpace(password))
					return null;

				return Authorization.Basic(username, password);
			}

			return null;
		}

		public AuthenticationHeaderValue AsHeader()
		{
			if (!String.IsNullOrWhiteSpace(Token))
				return new AuthenticationHeaderValue("Bearer", Token);

			byte[] bytes = Encoding.UTF8.GetBytes(String.Format("{0}:{1}", Username, Password));
			string encoded = Convert.ToBase64String(bytes);
			return new AuthenticationHeaderValue("Basic", encoded);
		}

		public override string ToString()
		{
			if (!String.IsNullOrWhiteSpace(Token))
				return String.Format("Bearer {0}", Token);

			return String.Format("Basic {0}:{1}", Username, Password);
		}
	}
}