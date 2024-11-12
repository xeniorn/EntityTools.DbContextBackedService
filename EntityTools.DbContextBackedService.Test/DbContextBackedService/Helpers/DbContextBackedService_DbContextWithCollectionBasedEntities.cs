

namespace EntityTools.DbContextBackedService.Test.DbContextBackedService.Helpers;

public class DbContextBackedService_DbContextWithCollectionBasedEntities(DbContextWithCollectionBasedEntities db)
    : DbContextBackedService<DbContextWithCollectionBasedEntities>(db)
{
    public Task<(IReadOnlyCollection<T> Exist, IReadOnlyCollection<T> Miss)> GetExistAndMiss<T>(List<T> input) where T : class
    {
        return GetExistingAndMissingEntitiesByValue(input);
    }
}