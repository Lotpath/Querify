namespace Querify
{
    public static class RepositoryExtensions
    {
        public static T GetOrThrow<T>(this IRepository repository, object id)
        {
            var entity = repository.Get<T>(id);
            if (entity == null)
            {
                throw new NoMatchFoundException("Expected a result but no matching result was found of type " +
                                                typeof(T));
            }
            return entity;
        }
    }
}