using System;

namespace Archon
{
	public static class UriExtensions
	{
		public static string UpToHost(this Uri uri)
		{
			if (uri == null)
				throw new ArgumentNullException(nameof(uri));

			string host = String.Format("{0}{1}{2}", uri.Scheme, Uri.SchemeDelimiter, uri.Authority);
			if (!host.EndsWith("/"))
				host += "/";

			return host;
		}

		public static string WithoutQuery(this Uri uri)
		{
			if (uri == null)
				throw new ArgumentNullException(nameof(uri));

			return String.Format("{0}{1}{2}{3}", uri.Scheme, Uri.SchemeDelimiter, uri.Authority, uri.AbsolutePath);
		}
	}
}