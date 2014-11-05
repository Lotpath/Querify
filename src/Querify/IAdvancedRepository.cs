using System.Linq;

namespace Querify
{
    public interface IAdvancedRepository
    {
        IQueryable<T> Query<T>();
    }
}