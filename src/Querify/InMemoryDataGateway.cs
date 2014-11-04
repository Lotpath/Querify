using System;
using System.Collections.Generic;
using System.Linq;

namespace Querify
{
    public class InMemoryDataGateway : IDataGateway
    {
        private readonly Dictionary<Type, IList<object>> _store;

        public InMemoryDataGateway()
        {
            _store = new Dictionary<Type, IList<object>>();
        }

        public void Save<T>(T entity)
        {
            var store = GetStore<T>();
            store.Add(entity);
        }

        public IQueryable<T> Query<T>()
        {
            var store = GetStore<T>();
            return store.Cast<T>().AsQueryable();
        }

        public void Delete<T>(T entity)
        {
            var store = GetStore<T>();
            store.Remove(entity);
        }

        private IList<object> GetStore<T>()
        {
            if (_store.ContainsKey(typeof (T)))
            {
                return _store[typeof (T)];
            }
            
            var store = new List<object>();
            _store[typeof (T)] = store;
            return store;
        }
    }
}