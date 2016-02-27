using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Archon.WebApi.Tests
{
	public class CsvModelBinderTests
	{
		readonly CsvModelBinder binder;

		public CsvModelBinderTests()
		{
			binder = new CsvModelBinder();
		}

		[Fact]
		public void can_convert_string_enumerable()
		{
			var items = binder.BindModel(typeof(IEnumerable<string>), "  homer ,marge ,bart,lisa,, ,   ") as IEnumerable<string>;

			Assert.NotNull(items);
			Assert.Equal(4, items.Count());
			Assert.Contains("homer", items);
			Assert.Contains("marge", items);
			Assert.Contains("bart", items);
			Assert.Contains("lisa", items);
		}

		[Fact]
		public void can_convert_string_array()
		{
			var items = binder.BindModel(typeof(string[]), "  homer ,marge ,bart,lisa,, ,   ") as string[];

			Assert.NotNull(items);
			Assert.Equal(4, items.Count());
			Assert.Contains("homer", items);
			Assert.Contains("marge", items);
			Assert.Contains("bart", items);
			Assert.Contains("lisa", items);
		}

		[Fact]
		public void can_convert_guid_enumerable()
		{
			Guid guid1 = Guid.NewGuid();
			Guid guid2 = Guid.NewGuid();

			var items = binder.BindModel(typeof(IEnumerable<Guid>), $"  {guid1} ,{guid2} ,, ,   ") as IEnumerable<Guid>;

			Assert.NotNull(items);
			Assert.Equal(2, items.Count());
			Assert.Contains(guid1, items);
			Assert.Contains(guid2, items);
		}

		[Fact]
		public void can_convert_guid_array()
		{
			Guid guid1 = Guid.NewGuid();
			Guid guid2 = Guid.NewGuid();

			var items = binder.BindModel(typeof(Guid[]), $"  {guid1} ,{guid2} ,, ,   ") as Guid[];

			Assert.NotNull(items);
			Assert.Equal(2, items.Count());
			Assert.Contains(guid1, items);
			Assert.Contains(guid2, items);
		}

		[Fact]
		public void can_not_blow_up_on_invalid_guids()
		{
			Guid guid1 = Guid.NewGuid();

			var items = binder.BindModel(typeof(IEnumerable<Guid>), $"this-is-not-a-valid-guid,{guid1},neither is this") as IEnumerable<Guid>;

			Assert.NotNull(items);
			Assert.Equal(1, items.Count());
			Assert.Contains(guid1, items);
		}

		[Fact]
		public void can_convert_integer_enumerable()
		{
			var items = binder.BindModel(typeof(IEnumerable<int>), "  13,  26 ,42,, ,   ") as IEnumerable<int>;

			Assert.NotNull(items);
			Assert.Equal(3, items.Count());
			Assert.Contains(13, items);
			Assert.Contains(26, items);
			Assert.Contains(42, items);
		}

		[Fact]
		public void can_convert_integer_array()
		{
			var items = binder.BindModel(typeof(int[]), "  13,  26 ,42,, ,   ") as int[];

			Assert.NotNull(items);
			Assert.Equal(3, items.Count());
			Assert.Contains(13, items);
			Assert.Contains(26, items);
			Assert.Contains(42, items);
		}

		[Fact]
		public void can_not_blow_up_on_invalid_integers()
		{
			var items = binder.BindModel(typeof(IEnumerable<int>), "this-is-not-a-valid-int,13,neither is this") as IEnumerable<int>;

			Assert.NotNull(items);
			Assert.Equal(1, items.Count());
			Assert.Contains(13, items);
		}

		[Theory]
		[InlineData(typeof(string))]
		[InlineData(typeof(int))]
		[InlineData(typeof(Guid))]
		[InlineData(typeof(object))]
		public void can_not_bind_with_non_enumerable_model(Type type)
		{
			var items = binder.BindModel(type, "hello,world");
			Assert.Null(items);
		}

		[Fact]
		public void can_bind_single_value()
		{
			var items = binder.BindModel(typeof(IEnumerable<string>), " homer.simpson  ") as IEnumerable<string>;

			Assert.NotNull(items);
			Assert.Equal(1, items.Count());
			Assert.Equal("homer.simpson", items.Single());
		}

		[Fact]
		public void can_handle_emtpy_args()
		{
			var items = binder.BindModel(typeof(IEnumerable<string>), "") as IEnumerable<string>;

			Assert.NotNull(items);
			Assert.Empty(items);
		}
	}
}