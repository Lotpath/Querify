using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate;
using NHibernate.Linq;
using Remotion.Linq;

namespace Querify
{
    public static class QueryableExtensions
    {
        public const int MaxPageSize = 128;
        public const int DefaultPageSize = 10;

        /// <summary>
        /// Find items matching the specified expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="expression"></param>
        /// <param name="page">Page number to retrieve</param>
        /// <param name="pageSize">Number of items per page to retrieve (default: 10, max: 128)</param>
        /// <returns>A paged set of matching results</returns>
        public static PagedResult<T> Find<T>(this IQueryable<T> query, Expression<Func<T, bool>> expression = null,
                                                int page = 1, int pageSize = DefaultPageSize)
        {
            if (pageSize < 1 || pageSize > MaxPageSize)
            {
                throw new ArgumentOutOfRangeException("pageSize", "Items per page must be a value between 1 and " + MaxPageSize);
            }

            if (!(page > 0))
            {
                throw new ArgumentOutOfRangeException("pageSize", "Page must be a value greater than zero");
            }

            if (expression != null)
            {
                query = query.Where(expression);
            }

            var count = AsFutureValue(query, x => x.Count());

            var items = AsFuture(query
                                     .Skip((page - 1) * pageSize)
                                     .Take(pageSize));

            return new PagedResult<T>(pageSize, () => count.Value, items);
        }

        /// <summary>
        /// Find one item matching the specified expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static Lazy<T> FindOne<T>(this IQueryable<T> query, Expression<Func<T, bool>> expression = null)
        {
            if (expression == null)
            {
                return new Lazy<T>(() => query.ToFutureValue().Value);
            }

            query = query.Where(expression);

            return new Lazy<T>(() => AsFutureValue(query).Value);
        }

        /// <summary>
        /// Find one item matching the specified expression, throwing a <see cref="NoMatchFoundException"/> of no match is found
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static Lazy<T> FindOneOrThrow<T>(this IQueryable<T> query, Expression<Func<T, bool>> expression = null)
            where T : class
        {
            var result = FindOne(query, expression);
            return new Lazy<T>(() =>
                {
                    var value = result.Value;
                    if (value == null)
                    {
                        throw new NoMatchFoundException("Expected a result but no matching result was found of type " +
                                                        typeof(T));
                    }
                    return value;
                });
        }

        private class FutureValue<T> : IFutureValue<T>
        {
            private readonly Func<T> _func;

            public FutureValue(Func<T> func)
            {
                _func = func;
            }

            public T Value
            {
                get { return _func(); }
            }
        }

        private static IEnumerable<T> AsFuture<T>(IQueryable<T> queryable)
        {
            if (queryable is QueryableBase<T>)
            {
                return queryable.ToFuture();
            }
            else
            {
                return queryable;
            }
        }

        private static IFutureValue<T> AsFutureValue<T>(IQueryable<T> queryable)
        {
            if (queryable is QueryableBase<T>)
            {
                return queryable.ToFutureValue();
            }
            else
            {
                return new FutureValue<T>(() => queryable.SingleOrDefault());
            }
        }

        private static IFutureValue<TResult> AsFutureValue<TSource, TResult>(IQueryable<TSource> queryable, Expression<Func<IQueryable<TSource>, TResult>> selector)
            where TResult : struct
        {
            if (queryable is QueryableBase<TSource>)
            {
                return queryable.ToFutureValue(selector);
            }
            else
            {
                return new FutureValue<TResult>(() => selector.Compile()(queryable));
            }
        }
    }
}