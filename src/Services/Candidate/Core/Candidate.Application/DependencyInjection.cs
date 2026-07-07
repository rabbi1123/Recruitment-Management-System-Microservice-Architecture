using Candidate.Application.Abstractions.Behaviors;
using Candidate.Application.Abstractions.CRUD;
using Candidate.Application.Abstractions.Data;
using Candidate.Application.Abstractions.Mapping;
using Candidate.Application.Abstractions.Validation;
using Candidate.Application.Common;
using Candidate.Application.Common.CRUD.Commands;
using Candidate.Application.Common.CRUD.Queries;
using Candidate.Application.Common.Data;
using Candidate.Application.Common.Validation;
using Common.Platform.Domain.Abstractions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Candidate.Application
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddApplication(this IServiceCollection services)
		{
			AddValidatorOptions();

			services.AddScoped<IValidationCatalog, ValidationCatalog>();

			services.AddScoped(typeof(IRequestValidator<>), typeof(RequestValidator<>));

			var assembly = typeof(DependencyInjection).Assembly;

			services.AddValidatorsFromAssembly(assembly);

			services.AddMediatR(configuration =>
			{
				configuration.RegisterServicesFromAssembly(assembly);
				configuration.AddOpenBehavior(typeof(RequestValidationBehavior<,>));
				configuration.AddOpenBehavior(typeof(RequestLoggingBehavior<,>));
				configuration.AddOpenBehavior(typeof(TransactionBehavior<,>));
				configuration.AddOpenBehavior(typeof(AuditUserBehavior<,>));
			});

			services.Scan(scan => scan
				.FromAssemblies(assembly)
				.AddClasses(classes => classes.AssignableToAny(
					typeof(ICreateEntityMapper<,>),
					typeof(IUpdateEntityMapper<,>),
					typeof(IResponseEntityMapper<,>)))
				.AsImplementedInterfaces()
				.WithScopedLifetime());

			AddGenericHandlersForEntities(services);

			services.AddScoped<IPostCommitActions, PostCommitActions>();

			return services;
		}

		private static bool ImplementsIGenericRequest(Type t)
		{
			return t.GetInterfaces()
				.Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IGenericRequest<>));
		}

		public static void AddGenericHandlersForEntities(IServiceCollection services)
		{
			var assembly = typeof(DependencyInjection).Assembly;

			var entityInterfaceAsmName = typeof(IEntity).Assembly.GetName().Name;

			var candidateAssemblies = AppDomain.CurrentDomain
				.GetAssemblies()
				.Where(a => !a.IsDynamic &&
							a.GetReferencedAssemblies().Any(r => r.Name == entityInterfaceAsmName));

			var entityTypes = candidateAssemblies
				.SelectMany(a => a.GetTypes())
				.Where(t => typeof(IEntity).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
				.ToArray();

			foreach (var entityType in entityTypes)
			{
				var entityName = entityType.Name;

				var createCommandType = assembly.GetTypes()
					.FirstOrDefault(t => !t.IsAbstract && !t.IsInterface && ImplementsIGenericRequest(t) &&
										 (t.Name == $"Create{entityName}Command" || t.Name == $"Add{entityName}Command"));

				var updateCommandType = assembly.GetTypes()
					.FirstOrDefault(t => !t.IsAbstract && !t.IsInterface && ImplementsIGenericRequest(t) &&
										 t.Name == $"Update{entityName}Command");

				var responseType = assembly.GetTypes()
					.Where(t => typeof(IGenericResponse).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
					.FirstOrDefault(t => t.Name == $"{entityName}Response");



				// CreateCommandHandler
				if (createCommandType != null)
				{
					services.AddScoped(
						typeof(IRequestHandler<,>).MakeGenericType(createCommandType, typeof(Result<CommandResponse>)),
						typeof(CreateCommandHandler<,>).MakeGenericType(createCommandType, entityType)
					);
				}

				// UpdateCommandHandler
				if (updateCommandType != null)
				{
					services.AddScoped(
						typeof(IRequestHandler<,>).MakeGenericType(updateCommandType, typeof(Result<CommandResponse>)),
						typeof(UpdateCommandHandler<,>).MakeGenericType(updateCommandType, entityType)
					);
				}

				// DeleteCommandHandler (always available)
				var deleteCommandType = typeof(DeleteCommand<>).MakeGenericType(entityType);
				services.AddScoped(
					typeof(IRequestHandler<,>).MakeGenericType(deleteCommandType, typeof(Result<CommandResponse>)),
					typeof(DeleteCommandHandler<,>).MakeGenericType(deleteCommandType, entityType)
				);


				if (responseType == null)
					continue; // Without a response type, queries don’t make sense.

				var listResponseType = typeof(List<>).MakeGenericType(responseType);
				var resultSingleType = typeof(Result<>).MakeGenericType(responseType);
				var resultListType = typeof(Result<>).MakeGenericType(listResponseType);

				// GetByIdQueryHandler
				var getByIdQueryType = typeof(GetByIdQuery<>).MakeGenericType(responseType);
				services.AddScoped(
					typeof(IRequestHandler<,>).MakeGenericType(getByIdQueryType, resultSingleType),
					typeof(GetByIdQueryHandler<,>).MakeGenericType(entityType, responseType)
				);

				// GetAllQueryHandler
				var getAllQueryType = typeof(GetAllQuery<>).MakeGenericType(responseType);
				services.AddScoped(
					typeof(IRequestHandler<,>).MakeGenericType(getAllQueryType, resultListType),
					typeof(GetAllQueryHandler<,>).MakeGenericType(entityType, responseType)
				);
			}
		}

		public static void AddValidatorOptions()
		{
			ValidatorOptions.Global.DisplayNameResolver = (type, member, expression) =>
			{
				// Prefer [Display(Name="...")]
				if (member != null)
				{
					var display = member.GetCustomAttribute<DisplayAttribute>()?.GetName();
					if (!string.IsNullOrWhiteSpace(display)) return display;
				}

				// Fallback to last segment
				var raw = member?.Name
					   ?? expression?.ToString()?.Split('.').LastOrDefault()
					   ?? string.Empty;

				// Trim "x => x.Prop"
				var idx = raw.IndexOf("=>", StringComparison.Ordinal);
				if (idx >= 0) raw = raw[(idx + 2)..].Trim().Split('.').LastOrDefault() ?? raw;

				return HumanizeToSentenceCase(raw);
			};

			static string HumanizeToSentenceCase(string s)
			{
				if (string.IsNullOrWhiteSpace(s)) return s;

				s = s.Replace('_', ' ').Replace('-', ' ');

				// Insert spaces: aA | AAa | a1 | 1a
				var spaced = Regex.Replace(
					s,
					@"(?<!^)(?:
                     (?<=\p{Ll})\p{Lu}|
                     (?<=\p{Lu})\p{Lu}(?=\p{Ll})|
                     (?<=\p{L})\p{N}|
                     (?<=\p{N})\p{L}
                  )",
					" $0",
					RegexOptions.IgnorePatternWhitespace);

				var words = spaced.Split(' ', StringSplitOptions.RemoveEmptyEntries);

				// Sentence case with acronym preservation:
				// first word: Capitalize; others: lower unless ALLCAPS (keep acronyms).
				for (int i = 0; i < words.Length; i++)
				{
					var w = words[i];
					var isAcronym = w.Length > 1 && w.ToUpperInvariant() == w;

					if (i == 0)
					{
						words[i] = isAcronym
							? w
							: char.ToUpper(w[0], CultureInfo.InvariantCulture) + w[1..].ToLower(CultureInfo.InvariantCulture);
					}
					else
					{
						words[i] = isAcronym ? w : w.ToLower(CultureInfo.InvariantCulture);
					}
				}
				return string.Join(' ', words);
			}
		}
	}
}
