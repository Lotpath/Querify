namespace Querify
{
    public interface IDataGateway : IQuerySource
    {
        void Save<T>(T entity);
        void Delete<T>(T entity);
    }
}