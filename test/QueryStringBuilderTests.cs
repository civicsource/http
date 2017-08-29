using Xunit;

namespace Archon.Http.Tests
{
	public class QueryStringBuilderTests
	{
		readonly QueryStringBuilder qs;

		public QueryStringBuilderTests()
		{
			qs = new QueryStringBuilder();
		}

		[Fact]
		public void can_build_empty_query_string()
		{
			Assert.Equal("", qs.ToString());
		}

		[Fact]
		public void can_build_query_string_with_single_item()
		{
			qs.Append("hello", "world");
			Assert.Equal("?hello=world", qs.ToString());
		}

		[Fact]
		public void can_build_query_string_with_multiple_items()
		{
			qs.Append("hello", "world");
			qs.Append("goodbye", "loneliness");
			Assert.Equal("?hello=world&goodbye=loneliness", qs.ToString());
		}

		[Fact]
		public void can_build_query_string_with_multiple_same_key_items()
		{
			qs.Append("hello", "world");
			qs.Append("hello", "universe");
			qs.Append("answer", "42");

			Assert.Equal("?hello=world&hello=universe&answer=42", qs.ToString());
		}
	}
}