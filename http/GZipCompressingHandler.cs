using System;
using System.Collections.Immutable;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Archon.Http
{
	/// <summary>
	/// GZip-compresses requests to save bytes over the wire.
	/// </summary>
	public sealed class GZipCompressingHandler : DelegatingHandler
	{
		private readonly ImmutableHashSet<HttpMethod> verbs;

		/// <inheritdoc cref="DelegatingHandler"/>
		/// <param name="verbsToCompress">A list of HTTP verbs to compress. Generally, only <see cref="HttpMethod.Post"/> and <see cref="HttpMethod.Put"/> are particularly useful.</param>
		public GZipCompressingHandler(HttpMessageHandler innerHandler, params HttpMethod[] verbsToCompress)
			: base(innerHandler)
		{
			if (verbsToCompress.Length == 0)
				throw new ArgumentException("Must specify at least one HTTP verb to compress", nameof(verbsToCompress));

			verbs = verbsToCompress.ToImmutableHashSet();
		}

		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
			CancellationToken cancellationToken)
		{
			if (verbs.Contains(request.Method) && request.Content != null)
				request.Content = new GZipHttpContent(request.Content);

			return base.SendAsync(request, cancellationToken);
		}
	}

	public sealed class GZipHttpContent : HttpContent
	{
		private readonly HttpContent originalContent;

		public GZipHttpContent(HttpContent content)
		{
			originalContent = content;

			foreach (var header in originalContent.Headers)
			{
				Headers.TryAddWithoutValidation(header.Key, header.Value);
			}

			Headers.ContentEncoding.Add("gzip");
		}

		protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
		{
			using (var gzip = new GZipStream(stream, CompressionLevel.Fastest, true))
			{
				await originalContent.CopyToAsync(gzip);
			}
		}

		protected override bool TryComputeLength(out long length)
		{
			length = -1;

			return false;
		}
	}
}
