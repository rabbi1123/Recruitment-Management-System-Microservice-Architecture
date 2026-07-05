using Common.Platform.Application.Abstractions;
using System;
using System.Data;

namespace Candidate.Application.Abstractions.Data
{
    public sealed class DbSession : IDbSession
    {
        private IUnitOfWork? _uow;

        public void Bind(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public IDbConnection Connection =>
            _uow?.Connection
            ?? throw new InvalidOperationException("DbSession not bound.");

        public IDbTransaction? Transaction =>
            _uow?.Transaction;


        public static IDbSession Create(IUnitOfWork uow)
        {
            var session = new DbSession();
            session.Bind(uow);
            return session;
        }
    }
}
