using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.ModelBinding;

namespace Archon.WebApi
{
	public class CsvModelBinder : IModelBinder
	{
		public Task<ModelBindingResult> BindModelAsync(ModelBindingContext ctx)
		{
			if (!typeof(IEnumerable).IsAssignableFrom(ctx.ModelType))
				return ModelBindingResult.NoResultAsync;

			string value = ctx.ValueProvider.GetValue(ctx.ModelName).FirstValue;

			if (String.IsNullOrWhiteSpace(value) || !value.Contains(","))
				return ModelBindingResult.NoResultAsync;

			var list = new ArrayList();

			Type elementType = ctx.ModelType.GetElementType() ?? ctx.ModelType.GetGenericArguments().FirstOrDefault() ?? typeof(string);

			string[] parts = value.Split(',');
			foreach (string item in parts)
				list.Add(Convert.ChangeType(item, elementType));

			return ModelBindingResult.SuccessAsync(ctx.ModelName, list.ToArray(elementType));
		}
	}
}