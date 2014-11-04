using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Querify.Advanced
{
    public static class AdvancedQueryableExtensions
    {
        public interface IAdvancedQueryable<out T>
        {
            IQueryable<T> Query { get; } 
        }

        private class AdvancedQueryableAdapter<T> : IAdvancedQueryable<T>
        {
            public AdvancedQueryableAdapter(IQueryable<T> query)
            {
                Query = query;
            }

            public IQueryable<T> Query { get; private set; } 
        }

        public static IAdvancedQueryable<T> Advanced<T>(this IQueryable<T> queryable)
        {
            return new AdvancedQueryableAdapter<T>(queryable);
        }

        /// <summary>
        /// Fetches all items of the specified type using the <value>QueryableExtensions.MaxPageSize</value> for paging
        /// </summary>
        /// <typeparam name="T">The type of the items to retrieve</typeparam>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static IEnumerable<T> FetchAll<T>(this IAdvancedQueryable<T> queryable)
        {
            return FetchAll(queryable, null);
        }

        /// <summary>
        /// Fetches all items of the specified type matching the supplied expression using the <value>QueryableExtensions.MaxPageSize</value> for paging
        /// </summary>
        /// <typeparam name="T">The type of the items to retrieve</typeparam>
        /// <param name="queryable"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static IEnumerable<T> FetchAll<T>(this IAdvancedQueryable<T> queryable, Expression<Func<T, bool>> expression)
        {
            var page = 1;
            var lastPage = 1;

            while (page <= lastPage)
            {
                var result = queryable.Query.Find(expression, page, QueryableExtensions.MaxPageSize);
                foreach (var item in result.Items)
                {
                    yield return item;
                }
                page++;
                lastPage = result.TotalPages;
            }
        }
    }
}
