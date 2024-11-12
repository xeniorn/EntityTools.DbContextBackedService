using EntityTools.DbContextBackedService.Test.Base;
using EntityTools.DbContextBackedService.Test.DbContextBackedService.Helpers;
using static EntityTools.DbContextBackedService.Test.DbContextBackedService.Helpers.DbContextWithValueBasedEntities;

namespace EntityTools.DbContextBackedService.Test.DbContextBackedService.ValueBasedEnts;

public class WithComplexPropertyWithConversion_NonTrivialEquality
    : PostgresBasedTest<DbContextWithValueBasedEntities, DbContextBackedService_DbContextWithValueBasedEntities>
{
    [Fact]
    public async Task GetExistingAndMissing_WithComplexPropertyWithConversion_HandlesCorrectlyIfNavPropertyIncluded()
    {
        var dummyProp1 = new NonTrivialEqualityRecord("1");
        var dummyProp2 = new NonTrivialEqualityRecord("2");
        var dummyProp3 = new NonTrivialEqualityRecord("3");

        var dummyProp1s = new NonTrivialEqualityRecord("1");
        var dummyProp2s = new NonTrivialEqualityRecord("Two");
        var dummyProp3s = new NonTrivialEqualityRecord("3");

        var np1 = new ValueBasedWithOwnedPropWithConversion_NonTrivialEquals(dummyProp1);
        var np2 = new ValueBasedWithOwnedPropWithConversion_NonTrivialEquals(dummyProp2);
        var np3 = new ValueBasedWithOwnedPropWithConversion_NonTrivialEquals(dummyProp3);

        var np1s = new ValueBasedWithOwnedPropWithConversion_NonTrivialEquals(dummyProp1s);
        var np2s = new ValueBasedWithOwnedPropWithConversion_NonTrivialEquals(dummyProp2s);
        var np3s = new ValueBasedWithOwnedPropWithConversion_NonTrivialEquals(dummyProp3s);


        var indb = new List<ValueBasedWithOwnedPropWithConversion_NonTrivialEquals>()
        {
            np1, np2
        };

        var search = new List<ValueBasedWithOwnedPropWithConversion_NonTrivialEquals>()
        {
            np2s, np3s
        };

        var exist = new List<ValueBasedWithOwnedPropWithConversion_NonTrivialEquals>()
        {
            np2
        };

        var miss = new List<ValueBasedWithOwnedPropWithConversion_NonTrivialEquals>()
        {
            np3s
        };

        await _db.ValueBasedWithOwnedPropWithConversion_NonTrivialEqualsSet.AddRangeAsync(indb);
        await _db.SaveChangesAsync();

        ResetState();
        var res = await _service.GetExistAndMiss(search);

        Assert.Equal(exist.OrderedIds(), res.Exist.OrderedIds());
        Assert.Equal(miss, res.Miss);
    }


}