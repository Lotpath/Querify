using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Querify
{
    public static class QueryableExtensions
    {
        public const int MaxPageSize = 128;
        public const int DefaultPageSize = 10;

        private static readonly List<IFutureConverter> FutureConverters 
            = new List<IFutureConverter>(); 

        private class LinqToObjectsFutureConverter : IFutureConverter
        {
            public bool CanHandle<T>(IQueryable<T> queryable)
            {
                return true;
            }

            public IEnumerable<T> ToFuture<T>(IQueryable<T> queryable)
            {
                return queryable;
            }

            public Lazy<T> ToFutureValue<T>(IQueryable<T> queryable)
            {
                return new Lazy<T>(() => queryable.SingleOrDefault());
            }

            public Lazy<TResult> ToFutureValue<TSource, TResult>(IQueryable<TSource> queryable, Expression<Func<IQueryable<TSource>, TResult>> selector)
            {
                return new Lazy<TResult>(() => selector.Compile()(queryable));
            }
        }

        static QueryableExtensions()
        {
            FutureConverters.Add(new LinqToObjectsFutureConverter());
        }

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
                throw new ArgumentOutOfRangeException("pageSize", 
                    string.Format(
                    "Items per page must be a value between 1 and {0} (default is {1}", MaxPageSize, DefaultPageSize));
            }

            if (!(page > 0))
            {
                throw new ArgumentOutOfRangeException("page", "Page must be a value greater than zero");
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
                return new Lazy<T>(() => AsFutureValue(query).Value);
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

        public static void RegisterConverter(IFutureConverter converter)
        {
            FutureConverters.Insert(0, converter);
        }

        private static IFutureConverter GetConverter<T>(IQueryable<T> queryable)
        {
            return FutureConverters.First(x => x.CanHandle(queryable));
        }

        private static IEnumerable<T> AsFuture<T>(IQueryable<T> queryable)
        {
            return GetConverter(queryable).ToFuture(queryable);
        }

        private static Lazy<T> AsFutureValue<T>(IQueryable<T> queryable)
        {
            return GetConverter(queryable).ToFutureValue(queryable);
        }

        private static Lazy<TResult> AsFutureValue<TSource, TResult>(IQueryable<TSource> queryable, Expression<Func<IQueryable<TSource>, TResult>> selector) where TResult : struct
        {
            return GetConverter(queryable).ToFutureValue(queryable, selector);
        }
    }
}