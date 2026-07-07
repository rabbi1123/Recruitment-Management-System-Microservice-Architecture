using Candidate.Application.Abstractions.Data;
using Candidate.Application.Abstractions.DomainEvents;
using Candidate.Domain.Abstractions;
using Common.Platform.Application.Abstractions;
using Common.Platform.Domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Candidate.Application.Abstractions.Behaviors
{
	public sealed class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
		where TRequest : IRequest<TResponse>
		where TResponse : Result
	{
		private readonly IUnitOfWorkFactory _uowFactory; // Scoped singleton per HTTP request / mediator scope
		private readonly ITransientErrorDetector _detector;
		private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;
		private readonly IPostCommitActions _postCommit;
		private readonly IDomainEventContext _domainEventContext;
		private readonly IDomainEventDispatcher _domainEventDispatcher;
		private readonly IDbSession _dbSession;

		public TransactionBehavior(IUnitOfWorkFactory uowFactory,
								   ITransientErrorDetector detector,
								   ILogger<TransactionBehavior<TRequest, TResponse>> logger,
								   IPostCommitActions postCommit,
								   IDomainEventContext domainEventContext,
								   IDomainEventDispatcher domainEventDispatcher,
								   IDbSession dbSession)
		{
			_uowFactory = uowFactory;
			_detector = detector;
			_logger = logger;
			_postCommit = postCommit;
			_domainEventContext = domainEventContext;
			_domainEventDispatcher = domainEventDispatcher;
			_dbSession = dbSession;
		}


		public async Task<TResponse> Handle(
			TRequest request,
			RequestHandlerDelegate<TResponse> next,
			CancellationToken ct)
		{
			bool isCommand = typeof(TRequest).Name.Contains("Command", StringComparison.Ordinal);
			bool isQuery = typeof(TRequest).Name.Contains("Query", StringComparison.Ordinal);

			if (isCommand) return await HandleCommand(next, request, ct);
			if (isQuery) return await HandleQuery(next, request, ct);

			// neither marked: run without opening anything (or choose one default)
			return await next();
		}

		// ---------- commands: WITH transaction + handler-level retry ----------
		private async Task<TResponse> HandleCommand(RequestHandlerDelegate<TResponse> next, TRequest request, CancellationToken ct)
		{
			string dbKey = (request as IUseDb)?.DbKey ?? DbKeys.Default;

			const int maxAttempts = 3;

			for (int attempt = 1; ; attempt++)
			{
				_domainEventContext.Clear();

				await using var uow = _uowFactory.Create(dbKey); // create fresh UoW per attempt

				var began = false;
				try
				{
					await uow.BeginAsync(IsolationLevel.ReadCommitted, ct);
					_dbSession.Bind(uow);
					began = true;

					var response = await next();

					if (response.IsFailure)
					{
						await uow.RollbackAsync(ct);
						return response;
					}

					await uow.CommitAsync(ct);

					await _postCommit.RunAsync(ct);

					var domainEvents = _domainEventContext.GetAll();

					if (domainEvents.Any())
					{
						_domainEventDispatcher.Enqueue(domainEvents);
						_domainEventContext.Clear();
					}

					return response;
				}
				catch (OperationCanceledException)
				{
					if (began) await uow.RollbackAsync(ct);
					throw;
				}
				catch (Exception ex) when (_detector.IsTransient(ex) && attempt < maxAttempts)
				{
					if (began) await uow.RollbackAsync(ct);

					_logger.LogWarning(ex, "Transient failure (attempt {Attempt}/{Max}) in {Request}. Retrying...",
						attempt, maxAttempts, typeof(TRequest).Name);

					int baseDelayMs = Math.Min(1000, 50 * (int)Math.Pow(2, attempt - 1)); // 50,100,200,400,800 capped
					int jitterMs = Random.Shared.Next(0, 100);
					try { await Task.Delay(baseDelayMs + jitterMs, ct); } catch (OperationCanceledException) { }
				}
				catch
				{
					if (began) await uow.RollbackAsync(ct);
					throw;
				}
			}
		}

		// ---------- queries: WITHOUT transaction (connection only) ----------
		private async Task<TResponse> HandleQuery(RequestHandlerDelegate<TResponse> next, TRequest request, CancellationToken ct)
		{
			string dbKey = (request as IUseDb)?.DbKey ?? DbKeys.Default;

			// optional: tiny retry only for opening the connection (not for the whole handler)
			const int maxAttempts = 2;

			for (int attempt = 1; ; attempt++)
			{
				await using var uow = _uowFactory.Create(dbKey); // create fresh UoW per attempt
				try
				{
					await uow.OpenConnectionAsync(ct);
					_dbSession.Bind(uow);
					return await next();
				}
				catch (OperationCanceledException) { throw; }
				catch (Exception ex) when (_detector.IsTransient(ex) && attempt < maxAttempts)
				{
					_logger.LogWarning(ex, "Transient open failure (attempt {Attempt}/{Max}) for {Request}. Retrying open...",
						attempt, maxAttempts, typeof(TRequest).Name);

					int delayMs = 50 + Random.Shared.Next(0, 40);
					try { await Task.Delay(delayMs, ct); } catch (OperationCanceledException) { }
				}
			}
		}
	}
}
