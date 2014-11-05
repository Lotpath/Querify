namespace Querify
{
    public interface IRepository
    {
        void Add<T>(T entity);
        T Get<T>(object id);
        void Remove<T>(T entity);
    }
}