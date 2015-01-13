using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate;
using NHibernate.Linq;
using Remotion.Linq;

namespace Querify
{
    public class NHibernateRepository : IRepository, IAdvancedRepository
    {
        private readonly ISession _session;

        static NHibernateRepository()
        {
            QueryableExtensions.RegisterConverter(new NHibernateFutureConverter());
            EagerFetchExtensions.RegisterFetchingProvider(new NHibernateEagerFetchProvider());
        }

        public NHibernateRepository(ISession session)
        {
            _session = session;
        }

        public void Add<T>(T entity)
        {
            _session.Save(entity);
        }

        public T Get<T>(object id)
        {
            return _session.Get<T>(id);
        }

        public void Remove<T>(T entity)
        {
            _session.Delete(entity);
        }

        public IAdvancedRepository Advanced { get { return this; } }

        IQueryable<T> IAdvancedRepository.Query<T>()
        {
            return _session.Query<T>();
        }

        private class NHibernateFutureConverter : IFutureConverter
        {
            public bool CanHandle<T>(IQueryable<T> queryable)
            {
                return queryable is QueryableBase<T>;
            }

            public IEnumerable<T> ToFuture<T>(IQueryable<T> queryable)
            {
                return queryable.ToFuture();
            }

            public Lazy<T> ToFutureValue<T>(IQueryable<T> queryable)
            {
                return new Lazy<T>(() => queryable.ToFutureValue().Value);
            }

            public Lazy<TResult> ToFutureValue<TSource, TResult>(IQueryable<TSource> queryable, Expression<Func<IQueryable<TSource>, TResult>> selector)
            {
                return new Lazy<TResult>(() => queryable.ToFutureValue(selector).Value);
            }
        }

        private class NHibernateEagerFetchProvider : IFetchingProvider
        {
            public bool CanHandle<T>(IQueryable<T> queryable)
            {
                return queryable is QueryableBase<T>;
            }

            public IFetchRequest<TOriginating, TRelated> Fetch<TOriginating, TRelated>(IQueryable<TOriginating> query, Expression<Func<TOriginating, TRelated>> relatedObjectSelector)
            {
                var fetch = EagerFetchingExtensionMethods.Fetch(query, relatedObjectSelector);
                return new FetchRequest<TOriginating, TRelated>(fetch);
            }

            public IFetchRequest<TOriginating, TRelated> FetchMany<TOriginating, TRelated>(IQueryable<TOriginating> query, Expression<Func<TOriginating, IEnumerable<TRelated>>> relatedObjectSelector)
            {
                var fetch = EagerFetchingExtensionMethods.FetchMany(query, relatedObjectSelector);
                return new FetchRequest<TOriginating, TRelated>(fetch);
            }

            public IFetchRequest<TQueried, TRelated> ThenFetch<TQueried, TFetch, TRelated>(IFetchRequest<TQueried, TFetch> query, Expression<Func<TFetch, TRelated>> relatedObjectSelector)
            {
                var impl = query as FetchRequest<TQueried, TFetch>;
                var fetch = EagerFetchingExtensionMethods.ThenFetch(impl.NhFetchRequest, relatedObjectSelector);
                return new FetchRequest<TQueried, TRelated>(fetch);
            }

            public IFetchRequest<TQueried, TRelated> ThenFetchMany<TQueried, TFetch, TRelated>(IFetchRequest<TQueried, TFetch> query, Expression<Func<TFetch, IEnumerable<TRelated>>> relatedObjectSelector)
            {
                var impl = query as FetchRequest<TQueried, TFetch>;
                var fetch = EagerFetchingExtensionMethods.ThenFetchMany(impl.NhFetchRequest, relatedObjectSelector);
                return new FetchRequest<TQueried, TRelated>(fetch);
            }
        }

        private class FetchRequest<TQueried, TFetch> : IFetchRequest<TQueried, TFetch>
        {
            public IEnumerator<TQueried> GetEnumerator()
            {
                return this.NhFetchRequest.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.NhFetchRequest.GetEnumerator();
            }

            public Type ElementType
            {
                get
                {
                    return this.NhFetchRequest.ElementType;
                }
            }

            public Expression Expression
            {
                get
                {
                    return this.NhFetchRequest.Expression;
                }
            }

            public IQueryProvider Provider
            {
                get
                {
                    return this.NhFetchRequest.Provider;
                }
            }

            public FetchRequest(INhFetchRequest<TQueried, TFetch> nhFetchRequest)
            {
                this.NhFetchRequest = nhFetchRequest;
            }

            public INhFetchRequest<TQueried, TFetch> NhFetchRequest { get; private set; }
        }
    }
}