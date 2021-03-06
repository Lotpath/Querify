﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq;
using Remotion.Linq;

namespace Querify
{
    internal class NHibernateEagerFetchProvider : IFetchingProvider
    {
        public bool CanHandle<T>(IQueryable<T> queryable)
        {
            return queryable is QueryableBase<T>
                || queryable is INHibernateFetchRequest<T>;
        }

        public IFetchRequest<TOriginating, TRelated> Fetch<TOriginating, TRelated>(IQueryable<TOriginating> query, Expression<Func<TOriginating, TRelated>> relatedObjectSelector)
        {
            var fetch = EagerFetchingExtensionMethods.Fetch(query, relatedObjectSelector);
            return new NHibernateFetchRequestAdapter<TOriginating, TRelated>(fetch);
        }

        public IFetchRequest<TOriginating, TRelated> FetchMany<TOriginating, TRelated>(IQueryable<TOriginating> query, Expression<Func<TOriginating, IEnumerable<TRelated>>> relatedObjectSelector)
        {
            var fetch = EagerFetchingExtensionMethods.FetchMany(query, relatedObjectSelector);
            return new NHibernateFetchRequestAdapter<TOriginating, TRelated>(fetch);
        }

        public IFetchRequest<TQueried, TRelated> ThenFetch<TQueried, TFetch, TRelated>(IFetchRequest<TQueried, TFetch> query, Expression<Func<TFetch, TRelated>> relatedObjectSelector)
        {
            var impl = query as NHibernateFetchRequestAdapter<TQueried, TFetch>;
            var fetch = EagerFetchingExtensionMethods.ThenFetch(impl.NhFetchRequest, relatedObjectSelector);
            return new NHibernateFetchRequestAdapter<TQueried, TRelated>(fetch);
        }

        public IFetchRequest<TQueried, TRelated> ThenFetchMany<TQueried, TFetch, TRelated>(IFetchRequest<TQueried, TFetch> query, Expression<Func<TFetch, IEnumerable<TRelated>>> relatedObjectSelector)
        {
            var impl = query as NHibernateFetchRequestAdapter<TQueried, TFetch>;
            var fetch = EagerFetchingExtensionMethods.ThenFetchMany(impl.NhFetchRequest, relatedObjectSelector);
            return new NHibernateFetchRequestAdapter<TQueried, TRelated>(fetch);
        }
    }
}