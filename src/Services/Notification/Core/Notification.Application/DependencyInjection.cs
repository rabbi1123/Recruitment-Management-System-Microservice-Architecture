using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Notification.Application.Abstractions.Behaviors;
using Notification.Application.Abstractions.Mapping;

namespace Notification.Application;

public static class DependencyInjection
{
	public static IServiceCollection AddApplication(this IServiceCollection services)
	{
		var assembly = typeof(DependencyInjection).Assembly;

		services.AddValidatorsFromAssembly(assembly);

		services.AddMediatR(configuration =>
		{
			configuration.RegisterServicesFromAssembly(assembly);
			configuration.AddOpenBehavior(typeof(RequestLoggingBehavior<,>));
			configuration.AddOpenBehavior(typeof(TransactionBehavior<,>));
		});

		services.Scan(scan => scan
			.FromAssemblies(assembly)
			.AddClasses(classes => classes.AssignableTo(typeof(IResponseEntityMapper<,>)))
			.AsImplementedInterfaces()
			.WithScopedLifetime());

		return services;
	}
}
