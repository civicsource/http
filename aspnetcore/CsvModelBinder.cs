using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Archon.AspNetCore
{
	public class CsvModelBinder : IModelBinder
	{
		static readonly MethodInfo generateListMethod = typeof(CsvModelBinder).GetMethod("GenerateList", BindingFlags.NonPublic | BindingFlags.Instance);

		public Task BindModelAsync(ModelBindingContext ctx)
		{
			var model = BindModel(ctx.ModelType, ctx.ValueProvider.GetValue(ctx.ModelName).FirstValue);
			ctx.Result = model == null ? ModelBindingResult.Failed() : ModelBindingResult.Success(model);
			return Task.FromResult<object>(null);
		}

		public IEnumerable BindModel(Type modelType, string value)
		{
			if (!typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(modelType.GetTypeInfo()))
				return null;

			Type elementType = modelType.GetElementType() ?? Enumerable.FirstOrDefault<Type>(modelType.GetGenericArguments());

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