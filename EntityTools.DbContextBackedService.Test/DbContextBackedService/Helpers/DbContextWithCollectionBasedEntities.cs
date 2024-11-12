using Microsoft.EntityFrameworkCore;

namespace EntityTools.DbContextBackedService.Test.DbContextBackedService.Helpers;

public class DbContextWithCollectionBasedEntities(DbContextOptions options) : DbContext(options)
{
    public DbSet<WithOneColl> WithOneCollSet { get; set; }
    public DbSet<WithOneColl> WithTwoCollSet { get; set; }


    public class WithOneColl : EntityBase
    {
        private WithOneColl()
        {
            
        }

        public WithOneColl(IReadOnlyCollection<DummyEnt> dummies)
        {
            Dummies1 = dummies.ToList();
        }

        [UsedInEntityByValueComparison]
        public ICollection<DummyEnt> Dummies1 { get; private set; } = [];
    }

    public class WithTwoColl : EntityBase
    {
        private WithTwoColl()
        {

        }

        public WithTwoColl(IReadOnlyCollection<DummyEnt> dummies1, IReadOnlyCollection<DummyEnt2> dummies2)
        {
            Dummies1 = dummies1.ToList();
            Dummies2 = dummies2.ToList();
        }

        [UsedInEntityByValueComparison]
        public ICollection<DummyEnt> Dummies1 { get; private set; } = [];
        [UsedInEntityByValueComparison]
        public ICollection<DummyEnt2> Dummies2 { get; private set; } = [];
    }

    public class DummyEnt : EntityBase
    {
        public DummyEnt()
        {
            
        }
    }

    public class DummyEnt2 : EntityBase
    {
        public DummyEnt2()
        {

        }
    }
}