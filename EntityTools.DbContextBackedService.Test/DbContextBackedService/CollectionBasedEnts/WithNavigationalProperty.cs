using EntityTools.DbContextBackedService.Test.Base;
using EntityTools.DbContextBackedService.Test.DbContextBackedService.Helpers;
using static EntityTools.DbContextBackedService.Test.DbContextBackedService.Helpers.DbContextWithCollectionBasedEntities;

namespace EntityTools.DbContextBackedService.Test.DbContextBackedService.CollectionBasedEnts;

public class WithNavigationalProperty
    : PostgresBasedTest<DbContextWithCollectionBasedEntities, DbContextBackedService_DbContextWithCollectionBasedEntities>
{
    [Fact]
    public async Task GetExistingAndMissing_HandlesCorrectly_OneColl()
    {
        var dummyProp1 = new DummyEnt();
        var dummyProp2 = new DummyEnt();
        var dummyProp3 = new DummyEnt();

        await _db.Set<DummyEnt>().AddRangeAsync([dummyProp1,dummyProp2]);
        await _db.SaveChangesAsync();

        var np1 = new WithOneColl([dummyProp1]);
        var np2 = new WithOneColl([dummyProp1, dummyProp2]);
        var np3 = new WithOneColl([dummyProp1, dummyProp2, dummyProp3]);

        var np1s = new WithOneColl([dummyProp1]);
        var np2s = new WithOneColl([dummyProp1, dummyProp2]);
        var np3s = new WithOneColl([dummyProp3, dummyProp2, dummyProp3]);
        

        var indb = new List<WithOneColl>()
        {
            np1, np2
        };

        var search = new List<WithOneColl>()
        {
            np2s, np3s
        };

        var exist = new List<WithOneColl>()
        {
            np2
        };

        var miss = new List<WithOneColl>()
        {
            np3s
        };

        await _db.WithOneCollSet.AddRangeAsync(indb);
        await _db.SaveChangesAsync();

        ResetState();
        var res = await _service.GetExistAndMiss(search);

        Assert.Equal(exist.OrderedIds(), res.Exist.OrderedIds());
        Assert.Equal(miss, res.Miss);
    }


}