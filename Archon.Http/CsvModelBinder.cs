using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.ModelBinding;

namespace Archon.Http
{
	public class CsvModelBinder : IModelBinder
	{
		static readonly MethodInfo generateListMethod = typeof(CsvModelBinder).GetMethod("GenerateList", BindingFlags.NonPublic | BindingFlags.Instance);

		public Task<ModelBindingResult> BindModelAsync(ModelBindingContext ctx)
		{
			var model = BindModel(ctx.ModelType, ctx.ValueProvider.GetValue(ctx.ModelName).FirstValue);

			if (model == null)
				return ModelBindingResult.NoResultAsync;

			return ModelBindingResult.SuccessAsync(ctx.ModelName, model);
		}

		public IEnumerable BindModel(Type modelType, string value)
		{
			if (!typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(modelType.GetTypeInfo()))
				return null;

			Type elementType = modelType.GetElementType() ?? modelType.GetGenericArguments().FirstOrDefault();

			if (elementType == null)
				return null;

			var method = generateListMethod.MakeGenericMethod(elementType);

			return (IEnumerable)method.Invoke(this, new object[] { value });
		}

		IEnumerable<T> GenerateList<T>(string values)
		{
			var list = new List<T>();

			if (!String.IsNullOrWhiteSpace(values))
			{
				string[] parts = values.Split(',');
				foreach (string item in parts)
				{
					if (!String.IsNullOrWhiteSpace(item))
					{
						T value = default(T);
						bool add = false;

						try
						{
							value = (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(item.Trim());
							add = true;
						}
						catch { /* swallow any formatting exceptions */ }

						if (add)
							list.Add(value);
					}
				}
			}

			return list.ToArray();
		}
	}
}