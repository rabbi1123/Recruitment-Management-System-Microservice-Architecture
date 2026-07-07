using Candidate.Infrastructure.Serialization;
using Candidate.WebAPI.Filters;
using Candidate.WebAPI.OpenApi;
using Microsoft.OpenApi.Models;
using System.Text.Json;

namespace Candidate.WebAPI
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
		{
			// ✅ CORS Configuration
			//var corsOriginsString = configuration["AllowedCorsOrigins:CorsOrigins"];
			//var allowedOrigins = string.IsNullOrEmpty(corsOriginsString)
			//	? Array.Empty<string>()
			//	: corsOriginsString
			//		.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

			//services.AddCors(options =>
			//{
			//	options.AddPolicy("AllowReactApp", builder =>
			//	{
			//		builder
			//			.WithOrigins(allowedOrigins)
			//			.AllowAnyHeader()
			//			.AllowAnyMethod()
			//			.AllowCredentials();
			//	});
			//});

			// ✅ Controllers
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

			// ✅ Swagger Configuration
			services.AddEndpointsApiExplorer();
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "Candidate API", Version = "v1" });
				c.CustomSchemaIds(type => type.FullName);

				c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					In = ParameterLocation.Header,
					Description = "Please enter JWT with Bearer into field (Bearer <token>)",
					Name = "Authorization",
					Type = SecuritySchemeType.ApiKey,
					Scheme = "Bearer"
				});

				c.AddSecurityRequirement(new OpenApiSecurityRequirement {
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = "Bearer"
							}
						},
						Array.Empty<string>()
					}
				});

				c.OperationFilter<ResultResponseExampleOperationFilter>();
			});

			return services;

		}

		public static IServiceCollection AddPresentationForTests(this IServiceCollection services)
		{
			// ✅ Controllers
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
			return services;
		}
	}
}
