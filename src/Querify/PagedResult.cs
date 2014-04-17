using System;
using System.Collections.Generic;

namespace Querify
{
    public class PagedResult<T>
    {
        private readonly int _pageSize;
        private readonly Func<int> _totalItemCount;
        private readonly IEnumerable<T> _items;

        public PagedResult(int pageSize, Func<int> totalItemCount, IEnumerable<T> items)
        {
            _pageSize = pageSize;
            _totalItemCount = totalItemCount;
            _items = items;
        }

        public int PageSize { get { return _pageSize; } }

        public int TotalPages
        {
            get
            {
                var count = _totalItemCount();
                return (int)Math.Ceiling(count / (decimal)_pageSize);
            }
        }

        public int TotalItems
        {
            get { return _totalItemCount(); }
        }

        public IEnumerable<T> Items { get { return _items; } }
    }
}