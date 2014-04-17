using System.Linq;
using NHibernate;
using NHibernate.Linq;

namespace Querify
{
    public interface IQuerySource
    {
        IQueryable<T> GetQuery<T>();
    }

    public class QuerySource : IQuerySource
    {
        private readonly ISession _session;

        public QuerySource(ISession session)
        {
            _session = session;
        }

        public IQueryable<T> GetQuery<T>()
        {
            return _session.Query<T>();
        }
    }
}
