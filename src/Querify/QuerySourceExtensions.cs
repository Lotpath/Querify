using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate;
using NHibernate.Linq;
using Remotion.Linq;

namespace Querify
{
    public static class QuerySourceExtensions
    {
        public static PagedResult<T> FindAll<T>(this IQueryable<T> query, ISpecification<T> specification = null, int page = 1, int pageSize = 10)
        {
            if (pageSize < 1 || pageSize > 128)
            {
                throw new PersistenceException("Items per page must be a value between 1 and 128");
            }

            if (!(page > 0))
            {
                throw new PersistenceException("Page must be a value greater than zero");
            }

            if (specification != null)
            {
                var condition = specification.IsSatisfiedBy();

                if (condition != null)
                {
                    query = query.Where(condition);
                }
            }

            var count = AsFutureValue(query, x => x.Count());

            var items = AsFuture(query
                                     .Skip((page - 1)*pageSize)
                                     .Take(pageSize));

            return new PagedResult<T>(pageSize, () => count.Value, items);
        }

        public static Func<T> FindOne<T>(this IQueryable<T> query, ISpecification<T> specification = null)
        {
            if (specification == null)
            {
                return () => query.ToFutureValue().Value;
            }

            var condition = specification.IsSatisfiedBy();

            if (condition != null)
            {
                query = query.Where(condition);
            }

            return () => AsFutureValue(query).Value;
        }

        public static Func<T> FindOneOrThrow<T>(this IQueryable<T> query, ISpecification<T> specification = null)
            where T : class
        {
            var result = FindOne(query, specification);
            return () =>
            {
                var value = result();
                if (value == null)
                {
                    throw new PersistenceException("Expected a result but no matching result was found of type " + typeof(T));
                }
                return value;
            };
        }

        private class FutureValue<T> : IFutureValue<T> 
        {
            private readonly Func<T> _func;

            public FutureValue(Func<T> func)
            {
                _func = func;
            }

            public T Value { get { return _func(); } }
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

        // enables application of count as a defered execution. should work by default in NH 4.0
        private static IFutureValue<TResult> ToFutureValue<TSource, TResult>(
             this IQueryable<TSource> source,
             Expression<Func<IQueryable<TSource>, TResult>> selector)
             where TResult : struct
        {
            var provider = (INhQueryProvider)source.Provider;
            var method = ((MethodCallExpression)selector.Body).Method;
            var expression = Expression.Call(null, method, source.Expression);
            return (IFutureValue<TResult>)provider.ExecuteFuture(expression);
        }
    }
}