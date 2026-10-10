using Common.Platform.Application.Abstractions;
using Common.Platform.Domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;
using Notification.Application.Abstractions.Data;
using System.Data;

namespace Notification.Application.Abstractions.Behaviors;

public sealed class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>
	where TResponse : Result
{
	private readonly IUnitOfWorkFactory _unitOfWorkFactory;
	private readonly ITransientErrorDetector _detector;
	private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;
	private readonly IDbSession _dbSession;

	public TransactionBehavior(
		IUnitOfWorkFactory unitOfWorkFactory,
		ITransientErrorDetector detector,
		ILogger<TransactionBehavior<TRequest, TResponse>> logger,
		IDbSession dbSession)
	{
		_unitOfWorkFactory = unitOfWorkFactory;
		_detector = detector;
		_logger = logger;
		_dbSession = dbSession;
	}

	public async Task<TResponse> Handle(
		TRequest request,
		RequestHandlerDelegate<TResponse> next,
		CancellationToken cancellationToken)
	{
		var isCommand = typeof(TRequest).Name.Contains("Command", StringComparison.Ordinal);
		var isQuery = typeof(TRequest).Name.Contains("Query", StringComparison.Ordinal);

		if (isCommand)
			return await HandleCommand(next, request, cancellationToken);

		if (isQuery)
			return await HandleQuery(next, request, cancellationToken);

		return await next();
	}

	private async Task<TResponse> HandleCommand(
		RequestHandlerDelegate<TResponse> next,
		TRequest request,
		CancellationToken cancellationToken)
	{
		var dbKey = (request as IUseDb)?.DbKey ?? DbKeys.Default;
		const int maxAttempts = 3;

		for (var attempt = 1; ; attempt++)
		{
			await using var unitOfWork = _unitOfWorkFactory.Create(dbKey);
			var began = false;

			try
			{
				await unitOfWork.BeginAsync(IsolationLevel.ReadCommitted, cancellationToken);
				_dbSession.Bind(unitOfWork);
				began = true;

				var response = await next();
				if (response.IsFailure)
				{
					await unitOfWork.RollbackAsync(cancellationToken);
					return response;
				}

				await unitOfWork.CommitAsync(cancellationToken);
				return response;
			}
			catch (OperationCanceledException)
			{
				if (began)
					await unitOfWork.RollbackAsync(cancellationToken);
				throw;
			}
			catch (Exception ex) when (_detector.IsTransient(ex) && attempt < maxAttempts)
			{
				if (began)
					await unitOfWork.RollbackAsync(cancellationToken);

				_logger.LogWarning(
					ex,
					"Transient failure (attempt {Attempt}/{Max}) in {Request}. Retrying...",
					attempt,
					maxAttempts,
					typeof(TRequest).Name);

				var delayMs = Math.Min(1000, 50 * (int)Math.Pow(2, attempt - 1)) + Random.Shared.Next(0, 100);
				await Task.Delay(delayMs, cancellationToken);
			}
			catch
			{
				if (began)
					await unitOfWork.RollbackAsync(cancellationToken);
				throw;
			}
		}
	}

	private async Task<TResponse> HandleQuery(
		RequestHandlerDelegate<TResponse> next,
		TRequest request,
		CancellationToken cancellationToken)
	{
		var dbKey = (request as IUseDb)?.DbKey ?? DbKeys.Default;
		const int maxAttempts = 2;

		for (var attempt = 1; ; attempt++)
		{
			await using var unitOfWork = _unitOfWorkFactory.Create(dbKey);

			try
			{
				await unitOfWork.OpenConnectionAsync(cancellationToken);
				_dbSession.Bind(unitOfWork);
				return await next();
			}
			catch (OperationCanceledException)
			{
				throw;
			}
			catch (Exception ex) when (_detector.IsTransient(ex) && attempt < maxAttempts)
			{
				_logger.LogWarning(
					ex,
					"Transient open failure (attempt {Attempt}/{Max}) for {Request}. Retrying open...",
					attempt,
					maxAttempts,
					typeof(TRequest).Name);

				await Task.Delay(50 + Random.Shared.Next(0, 40), cancellationToken);
			}
		}
	}
}
