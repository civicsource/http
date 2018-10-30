using Xunit;

namespace Archon.Http.Tests
{
	public class QueryStringBuilderTests
	{
		[Fact]
		public void can_build_empty_query_string()
		{
			var qs = new QueryStringBuilder();
			Assert.Equal("", qs.ToString());
		}

		[Fact]
		public void can_build_query_string_with_single_item()
		{
			var qs = new QueryStringBuilder();
			qs.Append("hello", "world");
			Assert.Equal("?hello=world", qs.ToString());
		}

		[Fact]
		public void can_build_query_string_with_single_item_without_value()
		{
			var qs = new QueryStringBuilder();
			qs.Append("hello");
			Assert.Equal("?hello", qs.ToString());
		}

		[Fact]
		public void can_build_query_string_with_multiple_items()
		{
			var qs = new QueryStringBuilder();
			qs.Append("hello", "world");
			qs.Append("goodbye", "loneliness");
			Assert.Equal("?hello=world&goodbye=loneliness", qs.ToString());
		}

		[Fact]
		public void can_build_query_string_with_multiple_same_key_items()
		{
			var qs = new QueryStringBuilder();
			qs.Append("hello", "world");
			qs.Append("hello", "universe");
			qs.Append("answer", "42");

			Assert.Equal("?hello=world&hello=universe&answer=42", qs.ToString());
		}

		[Fact]
		public void can_overwrite_query_parameters_with_set()
		{
			var qs = new QueryStringBuilder();
			qs.Set("hello", "world");
			qs.Set("hello", "universe");

			Assert.Equal("?hello=universe", qs.ToString());
		}

		[Fact]
		public void can_remove_query_parameters()
		{
			var qs = new QueryStringBuilder();
			qs.Append("hello", "world");
			qs.Append("hello", "universe");
			qs.Append("answer", "42");

			qs.Remove("hello");

			Assert.Equal("?answer=42", qs.ToString());
		}

		[Fact]
		public void can_url_encode_values()
		{
			var qs = new QueryStringBuilder();
			qs.Set("hello", "myname=homer & I like cheese");
			qs.Set("oh=hai", "nothai");

			Assert.Equal("?hello=myname%3Dhomer%20%26%20I%20like%20cheese&oh%3Dhai=nothai", qs.ToString());
		}

		[Fact]
		public void can_build_query_string_with_base_uri()
		{
			var qs = new QueryStringBuilder("https://example.com/");
			qs.Append("hello", "world");

			Assert.Equal("https://example.com/?hello=world", qs.ToString());
		}

		[Fact]
		public void can_build_empty_query_string_with_base_uri()
		{
			var qs = new QueryStringBuilder("https://example.com/");
			Assert.Equal("https://example.com/", qs.ToString());
		}

		[Theory]
		[InlineData("https://example.com/?oh=hai&mynameis=what&mynameis=slimshady", "https://example.com/?oh=what&mynameis=what&mynameis=slimshady")]
		[InlineData("https://example.com?doit", "https://example.com?doit&oh=what")]
		[InlineData("?doit", "?doit&oh=what")]
		public void can_parse_existing_query_string_from_base_uri(string baseUri, string expected)
		{
			var qs = new QueryStringBuilder(baseUri);
			qs.Set("oh", "what");

			Assert.Equal(expected, qs.ToString());
		}
	}
}