using EntityTools.DbContextBackedService.Test.Base;
using EntityTools.DbContextBackedService.Test.DbContextBackedService.Helpers;

namespace EntityTools.DbContextBackedService.Test.DbContextBackedService.ValueBasedEnts;

public class Basic
    : PostgresBasedTest<DbContextWithValueBasedEntities, DbContextBackedService_DbContextWithValueBasedEntities>
{
    [Fact]
    public Task InstantiationWorks()
    {
        return Task.CompletedTask;
    }

    private List<T> GenerateTypedList_Empty<T>() where T : EntityBase => [];

    private async Task GetExistingAndMissing_ReturnsEmptyForEmptyInput<T>() where T : EntityBase
    {
        var input = GenerateTypedList_Empty<T>();
        var res = await _service.GetExistAndMiss(input);

        Assert.Empty(res.Exist);
        Assert.Empty(res.Miss);
    }

    [Fact]
    public Task GetExistingAndMissing_ReturnsEmptyForEmptyInput_1()
        => GetExistingAndMissing_ReturnsEmptyForEmptyInput<DbContextWithValueBasedEntities.ValueBasedWithOneProp>();
    [Fact]
    public Task GetExistingAndMissing_ReturnsEmptyForEmptyInput_2()
        => GetExistingAndMissing_ReturnsEmptyForEmptyInput<DbContextWithValueBasedEntities.ValueBasedWithTwoProp>();
    [Fact]
    public Task GetExistingAndMissing_ReturnsEmptyForEmptyInput_1p1()
        => GetExistingAndMissing_ReturnsEmptyForEmptyInput<DbContextWithValueBasedEntities.ValueBasedWithOnePropAndOneExtra>();
}