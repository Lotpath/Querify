using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Querify
{
    public static class EagerFetchExtensions
    {
        private static readonly List<IFetchingProvider> FetchingProviders
             = new List<IFetchingProvider>();

        private class FakeFetchingProvider : IFetchingProvider
        {
            public bool CanHandle<T>(IQueryable<T> queryable)
            {
                return true;
            }

            public IFetchRequest<TOriginating, TRelated> Fetch<TOriginating, TRelated>(IQueryable<TOriginating> query,
                Expression<Func<TOriginating, TRelated>> relatedObjectSelector)
            {
                return new FetchRequest<TOriginating, TRelated>(query);
            }

            public IFetchRequest<TOriginating, TRelated> FetchMany<TOriginating, TRelated>(IQueryable<TOriginating> query,
                Expression<Func<TOriginating, IEnumerable<TRelated>>> relatedObjectSelector)
            {
                return new FetchRequest<TOriginating, TRelated>(query);
            }

            public IFetchRequest<TQueried, TRelated> ThenFetch<TQueried, TFetch, TRelated>(IFetchRequest<TQueried, TFetch> query,
                Expression<Func<TFetch, TRelated>> relatedObjectSelector)
            {
                var impl = query as FetchRequest<TQueried, TFetch>;
                return new FetchRequest<TQueried, TRelated>(impl.query);
            }

            public IFetchRequest<TQueried, TRelated> ThenFetchMany<TQueried, TFetch, TRelated>(IFetchRequest<TQueried, TFetch> query,
                Expression<Func<TFetch, IEnumerable<TRelated>>> relatedObjectSelector)
            {
                var impl = query as FetchRequest<TQueried, TFetch>;
                return new FetchRequest<TQueried, TRelated>(impl.query);
            }

            private class FetchRequest<TQueried, TFetch> : IFetchRequest<TQueried, TFetch>
            {
                public readonly IQueryable<TQueried> query;

                public IEnumerator<TQueried> GetEnumerator()
                {
                    return query.GetEnumerator();
                }

                System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
                {
                    return query.GetEnumerator();
                }

                public Type ElementType
                {
                    get { return query.ElementType; }
                }

                public Expression Expression
                {
                    get { return query.Expression; }
                }

                public IQueryProvider Provider
                {
                    get { return query.Provider; }
                }

                public FetchRequest(IQueryable<TQueried> query)
                {
                    this.query = query;
                }
            }
        }

        static EagerFetchExtensions()
        {
            FetchingProviders.Add(new FakeFetchingProvider());
        }

        public static IFetchRequest<TOriginating, TRelated> Fetch<TOriginating, TRelated>(
            this IQueryable<TOriginating> query, Expression<Func<TOriginating, TRelated>> relatedObjectSelector)
        {
            return GetProvider(query).Fetch(query, relatedObjectSelector);
        }

        public static IFetchRequest<TOriginating, TRelated> FetchMany<TOriginating, TRelated>(
            this IQueryable<TOriginating> query,
            Expression<Func<TOriginating, IEnumerable<TRelated>>> relatedObjectSelector)
        {
            return GetProvider(query).FetchMany(query, relatedObjectSelector);
        }

        public static IFetchRequest<TQueried, TRelated> ThenFetch<TQueried, TFetch, TRelated>(
            this IFetchRequest<TQueried, TFetch> query, Expression<Func<TFetch, TRelated>> relatedObjectSelector)
        {
            return GetProvider(query).ThenFetch(query, relatedObjectSelector);
        }

        public static IFetchRequest<TQueried, TRelated> ThenFetchMany<TQueried, TFetch, TRelated>(
            this IFetchRequest<TQueried, TFetch> query,
            Expression<Func<TFetch, IEnumerable<TRelated>>> relatedObjectSelector)
        {
            return GetProvider(query).ThenFetchMany(query, relatedObjectSelector);
        }

        public static void RegisterFetchingProvider(IFetchingProvider fetchingProvider)
        {
            FetchingProviders.Insert(0, fetchingProvider);
        }

        private static IFetchingProvider GetProvider<T>(IQueryable<T> queryable)
        {
            return FetchingProviders.First(x => x.CanHandle(queryable));
        }
    }
}