using System.Linq;

namespace Querify
{
    public interface IQuerySource
    {
        IQueryable<T> Query<T>();
    }
}
