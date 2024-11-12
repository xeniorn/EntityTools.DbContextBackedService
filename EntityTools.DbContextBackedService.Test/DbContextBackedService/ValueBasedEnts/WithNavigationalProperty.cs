using EntityTools.DbContextBackedService.Test.Base;
using EntityTools.DbContextBackedService.Test.DbContextBackedService.Helpers;
using static EntityTools.DbContextBackedService.Test.DbContextBackedService.Helpers.DbContextWithValueBasedEntities;

namespace EntityTools.DbContextBackedService.Test.DbContextBackedService.ValueBasedEnts;

public class WithNavigationalProperty
    : PostgresBasedTest<DbContextWithValueBasedEntities, DbContextBackedService_DbContextWithValueBasedEntities>
{
    [Fact]
    public async Task GetExistingAndMissing_WithNavigationProperty_HandlesCorrectlyIfNavPropertyIncluded()
    {
        var dummyProp1 = new DummyEnt();
        var dummyProp2 = new DummyEnt();
        var dummyProp3 = new DummyEnt();

        await _db.Set<DummyEnt>().AddRangeAsync([dummyProp1, dummyProp2]);
        await _db.SaveChangesAsync();

        var np1 = new ValueBasedWithNavProp(dummyProp1);
        var np2 = new ValueBasedWithNavProp(dummyProp2);
        var np3 = new ValueBasedWithNavProp(dummyProp3);

        var np1s = new ValueBasedWithNavProp(dummyProp1);
        var np2s = new ValueBasedWithNavProp(dummyProp2);
        var np3s = new ValueBasedWithNavProp(dummyProp3);

        List<ValueBasedWithNavProp> x = [np1s, np2s, np3s];
        x.ForEach(x=>x.TurnIntoDummy());

        var indb = new List<ValueBasedWithNavProp>()
        {
            np1, np2
        };

        var search = new List<ValueBasedWithNavProp>()
        {
            np2s, np3s
        };

        var exist = new List<ValueBasedWithNavProp>()
        {
            np2
        };

        var miss = new List<ValueBasedWithNavProp>()
        {
            np3s
        };

        await _db.ValueBasedWithNavPropSet.AddRangeAsync(indb);
        await _db.SaveChangesAsync();

        ResetState();
        var res = await _service.GetExistAndMiss(search);

        Assert.Equal(exist.OrderedIds(), res.Exist.OrderedIds());
        Assert.Equal(miss, res.Miss);
    }
}