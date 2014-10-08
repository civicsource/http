using System;

namespace Archon
{
	public static class UriExtensions
	{
		public static string WithoutQuery(this Uri uri)
		{
			if (uri == null)
				throw new ArgumentNullException("uri");

			return String.Format("{0}{1}{2}{3}", uri.Scheme, Uri.SchemeDelimiter, uri.Authority, uri.AbsolutePath);
		}
	}
}