using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Querify
{
    public class InMemoryRepository : IRepository, IAdvancedRepository
    {
        private readonly Dictionary<Type, Func<object, object>> _ids; 
        private readonly Dictionary<Type, IList<object>> _store;

        public InMemoryRepository()
        {
            _ids = new Dictionary<Type, Func<object, object>>();
            _store = new Dictionary<Type, IList<object>>();
        }

        public void ConfigureIdFor<T>(Func<T,object> idProperty)
        {
            _ids[typeof(T)] = x => idProperty((T)x);
        }

        public void Add<T>(T entity)
        {
            var store = GetStore<T>();
            store.Add(entity);
        }

        public T Get<T>(object id)
        {
            var property = GetIdProperty<T>();
            return (T)_store[typeof (T)].Single(x => property(x).Equals(id));
        }

        public void Remove<T>(T entity)
        {
            var store = GetStore<T>();
            store.Remove(entity);
        }

        public IAdvancedRepository Advanced { get { return this; } }

        IQueryable<T> IAdvancedRepository.Query<T>()
        {
            var store = GetStore<T>();
            return store.Cast<T>().AsQueryable();
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

        private Func<object, object> GetIdProperty<T>()
        {
            if (!_ids.ContainsKey(typeof (T)))
            {
                var property = typeof (T)
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .SingleOrDefault(x => x.Name == "Id");
                if (property == null)
                {
                    throw new InvalidOperationException(
                        string.Format(
                            "Could not determine Id property automatically. You must either have a property named 'Id' on your entity, or supply an Id property expression by calling 'ConfigureIdFor<{0}>()'",
                            typeof (T).Name));
                }
                _ids[typeof (T)] = property.GetValue;
            }
            return _ids[typeof (T)];
        }
    }
}