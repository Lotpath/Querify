using System.Linq;
using NHibernate;
using NHibernate.Linq;

namespace Querify
{
    public class NHibernateDataGateway : IDataGateway
    {
        private readonly ISession _session;

        public NHibernateDataGateway(ISession session)
        {
            _session = session;
        }

        public void Save<T>(T entity)
        {
            _session.Save(entity);
        }

        public IQueryable<T> Query<T>()
        {
            return _session.Query<T>();
        }

        public void Delete<T>(T entity)
        {
            _session.Delete(entity);
        }
    }
}