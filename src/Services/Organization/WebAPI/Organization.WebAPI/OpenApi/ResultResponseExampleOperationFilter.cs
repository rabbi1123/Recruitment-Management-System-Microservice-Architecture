using Common.Platform.Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections;
using System.Reflection;
using System.Text.Json;

namespace Organization.WebAPI.OpenApi
{
	public class ResultResponseExampleOperationFilter : IOperationFilter
	{
		private static readonly MethodInfo SuccessMethod = typeof(Result)
			.GetMethods()
			.First(m => m.Name == "Success" && m.IsGenericMethod && m.GetParameters().Length >= 1);

		private static readonly MethodInfo FailureMethod = typeof(Result)
			.GetMethods()
			.First(m => m.Name == "Failure" && m.IsGenericMethod && m.GetParameters().Length == 2);

		public void Apply(OpenApiOperation operation, OperationFilterContext context)
		{
			foreach (var (statusCode, response) in operation.Responses)
			{
				if (!response.Content.TryGetValue("application/json", out var media))
					continue;

				var producesAttr = context.MethodInfo
					.GetCustomAttributes<ProducesResponseTypeAttribute>()
					.FirstOrDefault(x => x.StatusCode.ToString() == statusCode);

				var responseType = producesAttr?.Type;

				if (responseType == null) continue;

				if (!IsResultType(responseType))
					continue;

				var innerType = responseType.GetGenericArguments()[0];

				var httpStatusCode = ParseStatusCode(statusCode);

				object? exampleObject = httpStatusCode == HttpResponseStatusCodes.Ok
					? CreateSuccessExample(context, innerType, httpStatusCode)
					: CreateFailureExample(innerType, httpStatusCode);

				// If we couldn't build an example (e.g. an un-constructable inner type),
				// skip rather than throw — Swagger generation must not crash the app.
				if (exampleObject == null)
					continue;

				var json = JsonSerializer.Serialize(exampleObject, new JsonSerializerOptions
				{
					DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never,
					WriteIndented = true,
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase
				});

				media.Example = new OpenApiString(json);
			}
		}

		private static bool IsResultType(Type type) =>
			type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Result<>);

		private static HttpResponseStatusCodes ParseStatusCode(string code)
		{
			return int.TryParse(code, out var intCode) &&
				   Enum.IsDefined(typeof(HttpResponseStatusCodes), intCode)
				? (HttpResponseStatusCodes)intCode
				: HttpResponseStatusCodes.BadRequest;
		}

		private object? CreateSuccessExample(OperationFilterContext ctx, Type innerType, HttpResponseStatusCodes code)
		{
			var schema = ctx.SchemaGenerator.GenerateSchema(innerType, ctx.SchemaRepository);

			var instance = CreateExampleObject(innerType);
			if (instance == null) return null;

			PopulateFromSchema(instance, schema);

			var method = SuccessMethod.MakeGenericMethod(innerType);
			return method.Invoke(null, new object?[] { instance, code });
		}

		private object? CreateFailureExample(Type innerType, HttpResponseStatusCodes code)
		{
			var error = CreateExampleObject(typeof(OpenApiCustomError));
			var method = FailureMethod.MakeGenericMethod(innerType);
			return method.Invoke(null, new object?[] { code, error });
		}

		private static object? CreateExampleObject(Type type, HashSet<Type>? visited = null)
		{
			visited ??= new HashSet<Type>();

			if (visited.Contains(type))
				return null;
			visited.Add(type);

			// Strings
			if (type == typeof(string))
				return "string";

			// Enums
			if (type.IsEnum)
				return Enum.GetValues(type).GetValue(0);

			// Nullable<T>
			if (Nullable.GetUnderlyingType(type) != null)
			{
				var t = Nullable.GetUnderlyingType(type)!;
				return CreateExampleObject(t, visited);
			}

			// Collections
			if (typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string))
			{
				var itemType = type.IsArray
					? type.GetElementType()
					: type.GetGenericArguments().FirstOrDefault();

				if (itemType == null) return null;

				var element = CreateExampleObject(itemType, visited);

				if (type.IsArray)
				{
					var arr = Array.CreateInstance(itemType, 1);
					arr.SetValue(element, 0);
					return arr;
				}

				var listType = typeof(List<>).MakeGenericType(itemType);
				var list = (IList)Activator.CreateInstance(listType)!;
				list.Add(element);
				return list;
			}

			// Complex types — construct via parameterless OR greediest ctor
			// (handles positional records like ReportDescriptorDto that have no
			//  parameterless constructor).
			var instance = TryCreateInstance(type, visited);
			if (instance == null) return null;

			// For types built by their parameterless ctor, fill writable props with
			// simple example values. (Positional records were already fully populated
			// by their constructor arguments inside TryCreateInstance.)
			if (HasParameterlessCtor(type))
			{
				foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
				{
					if (!prop.CanWrite) continue;

					object? defaultValue;

					if (type == typeof(OpenApiCustomError))
					{
						if (prop.Name.Equals(nameof(OpenApiCustomError.Message), StringComparison.OrdinalIgnoreCase))
							defaultValue = "string"; // keep message as example
						else if (prop.Name.Equals(nameof(OpenApiCustomError.Fields), StringComparison.OrdinalIgnoreCase))
							defaultValue = new Dictionary<string, string> { { "field1", "error" } };
						else
							defaultValue = DefaultFor(prop.PropertyType);
					}
					else
					{
						defaultValue = DefaultFor(prop.PropertyType);
					}

					prop.SetValue(instance, defaultValue);
				}
			}

			return instance;
		}

		private static object? DefaultFor(Type propertyType) =>
			propertyType.IsValueType
				? Activator.CreateInstance(propertyType)
				: propertyType == typeof(string)
					? "string"
					: null;

		private static bool HasParameterlessCtor(Type type) =>
			type.GetConstructor(Type.EmptyTypes) != null;

		/// <summary>
		/// Creates an instance using the parameterless constructor when available,
		/// otherwise the constructor with the most parameters (positional records),
		/// generating an example value for each constructor argument recursively.
		/// Returns null when the type cannot be constructed.
		/// </summary>
		private static object? TryCreateInstance(Type type, HashSet<Type> visited)
		{
			// 1. Parameterless ctor (classes, init-only POCOs)
			if (HasParameterlessCtor(type))
				return Activator.CreateInstance(type);

			// 2. Positional record / no default ctor — pick the greediest ctor.
			var greediest = type.GetConstructors()
				.OrderByDescending(c => c.GetParameters().Length)
				.FirstOrDefault();

			if (greediest == null) return null;

			var args = greediest.GetParameters()
				.Select(p => p.HasDefaultValue
					? p.DefaultValue
					: CreateExampleObject(p.ParameterType, visited))
				.ToArray();

			try
			{
				return greediest.Invoke(args);
			}
			catch
			{
				return null;
			}
		}

		private void PopulateFromSchema(object instance, OpenApiSchema schema)
		{
			if (instance == null || schema == null || schema.Properties == null)
				return;

			var type = instance.GetType();

			foreach (var kvp in schema.Properties)
			{
				var propName = kvp.Key;
				var propSchema = kvp.Value;

				var prop = type.GetProperty(propName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
				if (prop == null || !prop.CanWrite)
					continue;

				if (propSchema.Example != null)
				{
					var exampleValue = ConvertOpenApiAnyToClr(propSchema.Example, prop.PropertyType);
					prop.SetValue(instance, exampleValue);
				}
				else
				{
					// fallback default values if no example in schema
					if (prop.PropertyType == typeof(string))
					{
						prop.SetValue(instance, "string");
					}
					else if (prop.PropertyType == typeof(bool))
					{
						prop.SetValue(instance, true);
					}
					else if (prop.PropertyType.IsValueType)
					{
						prop.SetValue(instance, Activator.CreateInstance(prop.PropertyType));
					}
				}
			}
		}

		private object? ConvertOpenApiAnyToClr(IOpenApiAny openApiAny, Type targetType)
		{
			return openApiAny switch
			{
				OpenApiString s => s.Value,
				OpenApiInteger i => Convert.ChangeType(i.Value, targetType),
				OpenApiBoolean b => b.Value,
				OpenApiDouble d => Convert.ChangeType(d.Value, targetType),
				OpenApiFloat f => Convert.ChangeType(f.Value, targetType),
				_ => null,
			};
		}
	}
}