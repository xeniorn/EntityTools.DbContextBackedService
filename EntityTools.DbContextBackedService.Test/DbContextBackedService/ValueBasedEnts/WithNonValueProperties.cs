using EntityTools.DbContextBackedService.Test.Base;
using EntityTools.DbContextBackedService.Test.DbContextBackedService.Helpers;
using static EntityTools.DbContextBackedService.Test.DbContextBackedService.Helpers.DbContextWithValueBasedEntities;

namespace EntityTools.DbContextBackedService.Test.DbContextBackedService.ValueBasedEnts;

public class WithNonValueProperties
    : PostgresBasedTest<DbContextWithValueBasedEntities, DbContextBackedService_DbContextWithValueBasedEntities>
{
    [Fact]
    public async Task GetExistingAndMissing_WhenNonValuePropertyExists_RecognizesAsEqualEvenIfDifferent()
    {
        var v1_0 = new ValueBasedWithOnePropAndOneExtra(1, 0);
        var v2_0 = new ValueBasedWithOnePropAndOneExtra(2, 0);
        var v2_99 = new ValueBasedWithOnePropAndOneExtra(2, 99);
        var v3_0 = new ValueBasedWithOnePropAndOneExtra(3, 0);



        var indb = new List<ValueBasedWithOnePropAndOneExtra>()
        {
            v1_0, v2_0
        };

        var search = new List<ValueBasedWithOnePropAndOneExtra>()
        {
            v1_0, v2_99, v3_0
        };

        var exist = new List<ValueBasedWithOnePropAndOneExtra>()
        {
            v1_0, v2_0
        };

        var miss = new List<ValueBasedWithOnePropAndOneExtra>()
        {
            v3_0
        };

        await _db.ValueBasedWithOnePropAndOneExtraSet.AddRangeAsync(indb);
        await _db.SaveChangesAsync();

        ResetState();
        var res = await _service.GetExistAndMiss(search);

        Assert.Equal(exist.OrderedIds(), res.Exist.OrderedIds());
        Assert.Equal(miss, res.Miss);
    }


}