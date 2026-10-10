using Messaging.RabbitMq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notification.Infrastructure.Messaging.Consumers.Email;
using Notification.Infrastructure.Messaging.Consumers.Push;
using Notification.Infrastructure.Messaging.Consumers.Sms;

namespace Notification.Infrastructure.Messaging;

public static class MassTransitConfiguration
{
	public static IServiceCollection AddNotificationMessaging(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddRabbitMq(configuration, "notification", bus =>
		{
			bus.AddConsumer<EmployeeRegisteredEmailConsumer>();
			bus.AddConsumer<UserVerificationEmailConsumer>();
			bus.AddConsumer<OrganizationCreatedEmailConsumer>();
			bus.AddConsumer<JobPublishedEmailConsumer>();
			bus.AddConsumer<ApplicationSubmittedEmailConsumer>();
			bus.AddConsumer<OfferSentEmailConsumer>();

			bus.AddConsumer<EmployeeRegisteredSmsConsumer>();
			bus.AddConsumer<OrganizationCreatedSmsConsumer>();
			bus.AddConsumer<JobPublishedSmsConsumer>();
			bus.AddConsumer<ApplicationSubmittedSmsConsumer>();
			bus.AddConsumer<OfferSentSmsConsumer>();

			bus.AddConsumer<EmployeeRegisteredPushConsumer>();
			bus.AddConsumer<OrganizationCreatedPushConsumer>();
			bus.AddConsumer<JobPublishedPushConsumer>();
			bus.AddConsumer<ApplicationSubmittedPushConsumer>();
			bus.AddConsumer<OfferSentPushConsumer>();
		});

		return services;
	}
}
