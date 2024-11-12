using EntityTools.DbContextBackedService.Test.Base;
using EntityTools.DbContextBackedService.Test.DbContextBackedService.Helpers;
using static EntityTools.DbContextBackedService.Test.DbContextBackedService.Helpers.DbContextWithValueBasedEntities;

namespace EntityTools.DbContextBackedService.Test.DbContextBackedService.ValueBasedEnts;

public class DoubleValued
    : PostgresBasedTest<DbContextWithValueBasedEntities, DbContextBackedService_DbContextWithValueBasedEntities>
{
    [Fact]
    public async Task GetExistingAndMissing_ReturnsCorrectly_DoubleProp()
    {
        var input = Enumerable.Range(1, 3).Zip(Enumerable.Range(100, 3))
            .Select(x => new ValueBasedWithTwoProp() { ValueDefining1 = x.First, ValueDefining2 = x.Second })
            .ToList();

        var exist = input.Take(1).ToList();
        var miss = input.Except(exist).ToList();

        await _db.ValueBasedWithTwoPropSet.AddRangeAsync(exist);
        await _db.SaveChangesAsync();

        ResetState();
        var res = await _service.GetExistAndMiss(input);

        Assert.Equal(exist.OrderedIds(), res.Exist.OrderedIds());
        Assert.Equal(miss, res.Miss);
    }
}