using EntityTools.DbContextBackedService.Test.Base;
using EntityTools.DbContextBackedService.Test.DbContextBackedService.Helpers;
using static EntityTools.DbContextBackedService.Test.DbContextBackedService.Helpers.DbContextWithValueBasedEntities;

namespace EntityTools.DbContextBackedService.Test.DbContextBackedService.ValueBasedEnts;

public class WithComplexPropertyWithConversion
    : PostgresBasedTest<DbContextWithValueBasedEntities, DbContextBackedService_DbContextWithValueBasedEntities>
{
    [Fact]
    public async Task GetExistingAndMissing_WithComplexPropertyWithConversion_HandlesCorrectlyIfNavPropertyIncluded()
    {
        var dummyProp1 = new SimpleRecord("1");
        var dummyProp2 = new SimpleRecord("2");
        var dummyProp3 = new SimpleRecord("3");

        var dummyProp1s = new SimpleRecord("1");
        var dummyProp2s = new SimpleRecord("2");
        var dummyProp3s = new SimpleRecord("3");

        var np1 = new ValueBasedWithOwnedPropWithConversion(dummyProp1);
        var np2 = new ValueBasedWithOwnedPropWithConversion(dummyProp2);
        var np3 = new ValueBasedWithOwnedPropWithConversion(dummyProp3);

        var np1s = new ValueBasedWithOwnedPropWithConversion(dummyProp1s);
        var np2s = new ValueBasedWithOwnedPropWithConversion(dummyProp2s);
        var np3s = new ValueBasedWithOwnedPropWithConversion(dummyProp3s);


        var indb = new List<ValueBasedWithOwnedPropWithConversion>()
        {
            np1, np2
        };

        var search = new List<ValueBasedWithOwnedPropWithConversion>()
        {
            np2s, np3s
        };

        var exist = new List<ValueBasedWithOwnedPropWithConversion>()
        {
            np2
        };

        var miss = new List<ValueBasedWithOwnedPropWithConversion>()
        {
            np3s
        };

        await _db.ValueBasedWithOwnedPropWithConversionSet.AddRangeAsync(indb);
        await _db.SaveChangesAsync();

        ResetState();
        var res = await _service.GetExistAndMiss(search);

        Assert.Equal(exist.OrderedIds(), res.Exist.OrderedIds());
        Assert.Equal(miss, res.Miss);
    }


}