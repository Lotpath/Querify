using System.Collections.Generic;
using Querify;

namespace Sample.Domain.Acl
{
    public class PagedResponse<T>
    {
        public PagedResponse()
        {
            PageSize = QueryableExtensions.DefaultPageSize;
            Items = new List<T>();
        }

        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }

        public IList<T> Items { get; set; }
    }
}
