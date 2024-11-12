namespace EntityTools.DbContextBackedService.Test.DbContextBackedService.Helpers;

public class DbContextBackedService_DbContextWithValueBasedEntities(DbContextWithValueBasedEntities db)
    : DbContextBackedService<DbContextWithValueBasedEntities>(db)
{
    public Task<(IReadOnlyCollection<T> Exist, IReadOnlyCollection<T> Miss)> GetExistAndMiss<T>(List<T> input) where T : class
    {
        return GetExistingAndMissingEntitiesByValue(input);
    }
}