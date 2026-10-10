using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notification.Application.Abstractions.Data;
using Notification.Application.Interfaces;
using Notification.Infrastructure.Abstractions;
using Notification.Infrastructure.Data;
using Notification.Infrastructure.Email;
using Notification.Infrastructure.Messaging;
using Notification.Infrastructure.Persistence;
using Notification.Infrastructure.Push;
using Notification.Infrastructure.Sms;

namespace Notification.Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddNotificationMessaging(configuration);

		services
			.AddOptions<SmtpSettings>()
			.Bind(configuration.GetSection(SmtpSettings.SectionName))
			.ValidateDataAnnotations()
			.ValidateOnStart();

		services
			.AddOptions<PublicWebSettings>()
			.Bind(configuration.GetSection(PublicWebSettings.SectionName))
			.ValidateDataAnnotations()
			.ValidateOnStart();

		services.Configure<DbConnections>(options =>
		{
			var section = configuration.GetSection("ConnectionStrings");
			foreach (var child in section.GetChildren())
				options.ConnectionStrings[child.Key] = child.Value!;
		});

		services.AddScoped<IDbSession, DbSession>();
		services.AddScoped<IDbExecutor, DbExecutor>();
		services.AddScoped<IUnitOfWorkFactory, SqlUnitOfWorkFactory>();

		Common.DataAccessService.DependencyInjection.AddInfrastructure(services, configuration);

		services.AddScoped<INotificationRepository, NotificationRepository>();
		services.AddSingleton<IEmailTemplateService, EmailTemplateService>();
		services.AddScoped<IEmailService, SmtpEmailService>();
		services.AddScoped<ISmsService, SmsService>();
		services.AddScoped<IPushService, PushService>();

		return services;
	}
}
