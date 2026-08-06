using Candidate.Application.Abstractions.Auth;
using Candidate.Application.Abstractions.Data;
using Candidate.Application.Abstractions.DomainEvents;
using Candidate.Application.Abstractions.Files;
using Candidate.Application.Abstractions.Images;
using Candidate.Application.Common.Options;
using Candidate.Domain.Abstractions;
using Candidate.Infrastructure.Abstractions;
using Candidate.Infrastructure.Auth;
using Candidate.Infrastructure.Data;
using Candidate.Infrastructure.DomainEvents;
using Candidate.Infrastructure.Files;
using Candidate.Infrastructure.Images;
using Candidate.Infrastructure.Messaging;
using Candidate.Infrastructure.Repositories;
using Common.Platform.Domain.Abstractions;
using Dapper;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Candidate.Infrastructure
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
		{
			services
				.AddOptions<JwtSettings>()
				.BindConfiguration("JwtSettings")
				.ValidateDataAnnotations()
				.ValidateOnStart();

			services
				.AddOptions<RabbitMqSettings>()
				.Bind(configuration.GetSection(RabbitMqSettings.SectionName))
				.ValidateDataAnnotations()
				.ValidateOnStart();

			var rabbitMqSettings = configuration.GetSection(RabbitMqSettings.SectionName).Get<RabbitMqSettings>()
				?? new RabbitMqSettings();

			services.AddMassTransit(x =>
			{
				x.AddConsumer<EmployeeRegisteredConsumer>();

				x.UsingRabbitMq((context, cfg) =>
				{
					cfg.Host(rabbitMqSettings.Host, h =>
					{
						h.Username(rabbitMqSettings.Username);
						h.Password(rabbitMqSettings.Password);
					});

					cfg.ConfigureEndpoints(context);
				});
			});

			services.Configure<DbConnections>(options =>
			{
				var section = configuration.GetSection("ConnectionStrings");

				foreach (var child in section.GetChildren())
				{
					options.ConnectionStrings[child.Key] = child.Value!;
				}
			});

			services.AddScoped<IDbSession, DbSession>();
			services.AddScoped<IDbExecutor, DbExecutor>();
			services.AddScoped<IUnitOfWorkFactory, SqlUnitOfWorkFactory>();

			services.AddHttpClient();
			services.AddHttpContextAccessor();
			services.AddScoped<ICurrentUserService, CurrentUserService>();

			services.AddScoped<IGenericSearchRepository, GenericSearchRepository>();

			Common.DataAccessService.DependencyInjection.AddInfrastructure(services, configuration);

			var assembly = typeof(DependencyInjection).Assembly;

			AddGenericDapperTypeHandler();

			services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

			// ✅ JWT Authentication
			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
					.AddJwtBearer(options =>
					{
						var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();

						// ✅ This part ensures token comes from the cookie
						options.Events = new JwtBearerEvents
						{
							OnMessageReceived = context =>
							{
								var token = context.Request.Cookies["AccessToken"];
								if (!string.IsNullOrEmpty(token))
								{
									context.Token = token;
								}
								return Task.CompletedTask;
							},

							OnChallenge = context =>
							{
								// override default 401 to return custom JSON
								context.HandleResponse();

								context.Response.StatusCode = StatusCodes.Status401Unauthorized;
								context.Response.ContentType = "application/json";

								var errorResponse = Result.Failure(HttpResponseStatusCodes.Unauthorized, Error.Unauthorized());

								return context.Response.WriteAsJsonAsync(errorResponse);
							}
						};

						options.TokenValidationParameters = new TokenValidationParameters
						{
							ValidateIssuer = true,
							ValidateAudience = true,
							ValidateLifetime = true,
							ValidateIssuerSigningKey = true,
							ValidIssuer = jwtSettings.Issuer,
							ValidAudience = jwtSettings.Audience,
							IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
							ClockSkew = TimeSpan.Zero // Optional: prevents 5-minute default grace
						};
					});


			AddImageValidationFromConfig(services, configuration);


			services.Configure<AppSettings>(configuration);

			services
				.AddOptions<FileStorageOptions>()
				.BindConfiguration("Files")
				.ValidateDataAnnotations()
				.ValidateOnStart();

			services
				.AddOptions<ApplicantInvitationSettings>()
				.BindConfiguration("ApplicantInvitationSettings")
				.ValidateDataAnnotations()
				.ValidateOnStart();

			services.AddScoped<IFileStorageService, FileSystemStorageService>();
			services.AddSingleton<IFileUploadPolicy, FileUploadPolicy>();


			services.AddScoped<IDomainEventContext, DomainEventContext>();
			services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
			services.AddSingleton<IBackgroundEventQueue, BackgroundEventQueue>();
			services.AddHostedService<BackgroundDomainEventWorker>();

			return services;
		}

		public static void AddGenericDapperTypeHandler()
		{
			var asm = typeof(IValueObject<>).Assembly;

			var voTypes = asm.GetTypes()
							.Where(t => !t.IsAbstract && !t.IsInterface)
							.Where(t => t.GetInterfaces().Any(i =>
								i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValueObject<>)))
							.ToList();

			foreach (var voType in voTypes)
			{
				var handlerType = typeof(GenericDapperTypeHandler<>).MakeGenericType(voType);

				var handlerInstance = Activator.CreateInstance(handlerType)
								?? throw new InvalidOperationException($"Could not create handler for {voType.FullName}");

				// cast to the non-generic ITypeHandler that Dapper understands
				var iTypeHandler = handlerInstance as SqlMapper.ITypeHandler
					?? throw new InvalidOperationException($"Handler {handlerType.FullName} does not implement SqlMapper.ITypeHandler");

				// SqlMapper.AddTypeHandler accepts object of TypeHandler<T>
				SqlMapper.AddTypeHandler(voType, iTypeHandler);
			}
		}


		public static IServiceCollection AddImageValidationFromConfig(this IServiceCollection services, IConfiguration config)
		{
			services.Configure<ImageValidationOptions>(config.GetSection("ImageValidation"));

			services
				.AddOptions<ImageValidationOptions>()
				.BindConfiguration("ImageValidation")
				.ValidateDataAnnotations()
				.ValidateOnStart();

			services.AddSingleton<IImageValidator, ImageValidator>();

			return services;
		}
	}
}
