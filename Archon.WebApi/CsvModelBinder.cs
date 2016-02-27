using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.ModelBinding;

namespace Archon.WebApi
{
	public class CsvModelBinder : IModelBinder
	{
		static readonly MethodInfo generateListMethod = typeof(CsvModelBinder).GetMethod("GenerateList", BindingFlags.NonPublic);

		public Task<ModelBindingResult> BindModelAsync(ModelBindingContext ctx)
		{
			if (!typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(ctx.ModelType.GetTypeInfo()))
				return ModelBindingResult.NoResultAsync;

			string value = ctx.ValueProvider.GetValue(ctx.ModelName).FirstValue;

			if (String.IsNullOrWhiteSpace(value) || !value.Contains(","))
				return ModelBindingResult.NoResultAsync;

			Type elementType = ctx.ModelType.GetElementType() ?? ctx.ModelType.GetGenericArguments().FirstOrDefault() ?? typeof(string);

			var method = generateListMethod.MakeGenericMethod(elementType);

			IEnumerable list = (IEnumerable)method.Invoke(this, new object[0]);
			return ModelBindingResult.SuccessAsync(ctx.ModelName, list);
		}

		IEnumerable<T> GenerateList<T>(string value)
		{
			var list = new List<T>();

			string[] parts = value.Split(',');
			foreach (string item in parts)
				list.Add((T)Convert.ChangeType(item, typeof(T)));

			return list;
		}
	}
}