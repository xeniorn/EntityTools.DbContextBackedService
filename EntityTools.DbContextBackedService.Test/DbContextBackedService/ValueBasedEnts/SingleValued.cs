using EntityTools.DbContextBackedService.Test.Base;
using EntityTools.DbContextBackedService.Test.DbContextBackedService.Helpers;
using static EntityTools.DbContextBackedService.Test.DbContextBackedService.Helpers.DbContextWithValueBasedEntities;

namespace EntityTools.DbContextBackedService.Test.DbContextBackedService.ValueBasedEnts;

public class SingleValued
    : PostgresBasedTest<DbContextWithValueBasedEntities, DbContextBackedService_DbContextWithValueBasedEntities>
{
    [Fact]
    public async Task GetExistingAndMissing_ReturnsCorrectly_SingleProp()
    {
        var input = Enumerable.Range(1, 3)
            .Select(x => new ValueBasedWithOneProp() { ValueDefining1 = x })
            .ToList();

        var exist = input.Take(1).ToList();
        var miss = input.Except(exist).ToList();

        await _db.ValueBasedWithOnePropSet.AddRangeAsync(exist);
        await _db.SaveChangesAsync();

        ResetState();
        var res = await _service.GetExistAndMiss(input);

        Assert.Equal(exist.OrderedIds(), res.Exist.OrderedIds());
        Assert.Equal(miss, res.Miss);
    }

}