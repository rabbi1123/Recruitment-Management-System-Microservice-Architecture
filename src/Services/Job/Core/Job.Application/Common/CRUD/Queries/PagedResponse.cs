using System.Collections.Generic;

namespace Job.Application.Common.CRUD.Queries
{
    public class PagedResponse<T>
    {
        public int Count { get; set; }
        public IEnumerable<T> List { get; set; }
    }
}
