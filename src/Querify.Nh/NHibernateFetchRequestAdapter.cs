using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq;

namespace Querify
{
    internal interface INHibernateFetchRequest<T>
    {        
    }

    internal class NHibernateFetchRequestAdapter<TQueried, TFetch> : IFetchRequest<TQueried, TFetch>, INHibernateFetchRequest<TQueried>
    {
        public IEnumerator<TQueried> GetEnumerator()
        {
            return NhFetchRequest.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.NhFetchRequest.GetEnumerator();
        }

        public Type ElementType
        {
            get
            {
                return NhFetchRequest.ElementType;
            }
        }

        public Expression Expression
        {
            get
            {
                return NhFetchRequest.Expression;
            }
        }

        public IQueryProvider Provider
        {
            get
            {
                return NhFetchRequest.Provider;
            }
        }

        public NHibernateFetchRequestAdapter(INhFetchRequest<TQueried, TFetch> nhFetchRequest)
        {
            NhFetchRequest = nhFetchRequest;
        }

        public INhFetchRequest<TQueried, TFetch> NhFetchRequest { get; private set; }
    }
}