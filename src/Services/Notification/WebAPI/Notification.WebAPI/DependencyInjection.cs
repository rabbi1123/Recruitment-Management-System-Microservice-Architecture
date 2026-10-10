using Microsoft.OpenApi.Models;
using Notification.WebAPI.Filters;
using Notification.WebAPI.Serialization;
using System.Text.Json;

namespace Notification.WebAPI;

public static class DependencyInjection
{
	public static IServiceCollection AddPresentation(this IServiceCollection services)
	{
		services.AddControllers(options =>
		{
			options.Filters.Add<ThrowOnInvalidModelStateActionFilter>();
		})
		.AddJsonOptions(options =>
		{
			options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
			options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
			options.JsonSerializerOptions.Converters.Add(new TrimmingStringJsonConverter());
		})
		.ConfigureApiBehaviorOptions(options =>
		{
			options.SuppressModelStateInvalidFilter = true;
		});

		services.AddEndpointsApiExplorer();
		services.AddSwaggerGen(c =>
		{
			c.SwaggerDoc("v1", new OpenApiInfo { Title = "Notification API", Version = "v1" });
			c.CustomSchemaIds(type => type.FullName);
		});

		return services;
	}
}
