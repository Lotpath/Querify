using System.Linq;
using NHibernate;
using NHibernate.Linq;

namespace Querify
{
    public class NHibernateRepository : IRepository, IQuerySource
    {
        private readonly ISession _session;

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

        public IQueryable<T> Query<T>()
        {
            return _session.Query<T>();
        }
    }
}