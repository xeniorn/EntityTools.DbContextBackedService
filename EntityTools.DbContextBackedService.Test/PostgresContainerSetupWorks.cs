using System.ComponentModel.DataAnnotations;
using EntityTools.DbContextBackedService.Test.Base;
using Microsoft.EntityFrameworkCore;

namespace EntityTools.DbContextBackedService.Test;

public class PostgresContainerSetupWorks 
    : PostgresBasedTest<PostgresContainerSetupWorks.DummyDbContext>
{
    public class DummyDbContext : DbContext
    {
        public DbSet<MyEntity> MyEntities { get; set; }

        public DummyDbContext() : base()
        {
        }

        public DummyDbContext(DbContextOptions options) : base(options)
        {
        }

        public class MyEntity
        {
            [Key]
            public string Name { get; set; }
        }
    }

    [Fact]
    public Task DoesNotCrashBeforeAnyInput()
    {
        // simply no error happens
        return Task.CompletedTask;
    }

    [Fact]
    public async Task CanAddAndFetchItemFromDummyDb()
    {
        // Arrange
        var newItem = new DummyDbContext.MyEntity { Name = "Test Item" };

        // Act
        _db.MyEntities.Add(newItem);
        await _db.SaveChangesAsync();

        var retrievedItem = await _db.MyEntities.FirstOrDefaultAsync(i => i.Name == "Test Item");

        // Assert
        Assert.NotNull(retrievedItem);
        Assert.Equal("Test Item", retrievedItem.Name);
    }
}