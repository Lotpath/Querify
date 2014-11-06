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
        
    }
}