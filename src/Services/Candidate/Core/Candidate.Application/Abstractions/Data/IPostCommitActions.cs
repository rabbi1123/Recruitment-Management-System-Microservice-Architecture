using System;
using System.Threading;
using System.Threading.Tasks;

namespace Candidate.Application.Abstractions.Data
{
    public interface IPostCommitActions
    {
        void Add(Func<CancellationToken, Task> action);
        Task RunAsync(CancellationToken ct);
    }
}
