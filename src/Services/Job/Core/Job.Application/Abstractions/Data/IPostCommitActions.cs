using System;
using System.Threading;
using System.Threading.Tasks;

namespace Job.Application.Abstractions.Data
{
    public interface IPostCommitActions
    {
        void Add(Func<CancellationToken, Task> action);
        Task RunAsync(CancellationToken ct);
    }
}
