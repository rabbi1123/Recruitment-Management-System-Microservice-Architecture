using Organization.Application.Abstractions.Data;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Organization.Application.Common.Data
{
    public sealed class PostCommitActions : IPostCommitActions
    {
        private readonly List<Func<CancellationToken, Task>> _actions = new();

        public void Add(Func<CancellationToken, Task> action) => _actions.Add(action);

        public async Task RunAsync(CancellationToken ct)
        {
            foreach (var action in _actions) await action(ct);
            _actions.Clear();
        }
    }
}
