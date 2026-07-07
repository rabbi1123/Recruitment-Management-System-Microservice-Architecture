using System.Collections.Generic;

namespace Candidate.Application.Common.CRUD.Queries
{
    public class PagedResponse<T>
    {
        public int Count { get; set; }
        public IEnumerable<T> List { get; set; }
    }
}
