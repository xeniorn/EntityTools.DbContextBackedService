using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EntityTools.DbContextBackedService.Test.DbContextBackedService.Helpers;

public class DbContextWithValueBasedEntities(DbContextOptions options) : DbContext(options)
{
    public DbSet<ValueBasedWithOneProp> ValueBasedWithOnePropSet { get; set; }
    public DbSet<ValueBasedWithTwoProp> ValueBasedWithTwoPropSet { get; set; }
    public DbSet<ValueBasedWithOnePropAndOneExtra> ValueBasedWithOnePropAndOneExtraSet { get; set; }
    public DbSet<ValueBasedWithNavProp> ValueBasedWithNavPropSet { get; set; }
    public DbSet<ValueBasedWithOwnedPropWithConversion> ValueBasedWithOwnedPropWithConversionSet { get; set; }
    public DbSet<ValueBasedWithOwnedPropWithConversion_NonTrivialEquals> ValueBasedWithOwnedPropWithConversion_NonTrivialEqualsSet { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ValueBasedWithOwnedPropWithConversion>()
            .Property(x => x.WithConvert)
            .HasConversion<SimpleRecord.StringConverter>();

        modelBuilder.Entity<ValueBasedWithOwnedPropWithConversion_NonTrivialEquals>()
            .Property(x => x.WithConvert)
            .HasConversion<NonTrivialEqualityRecord.StringConverter>();

    }

    public class ValueBasedWithOneProp : EntityBase
    {
        [UsedInEntityByValueComparison]
        public int ValueDefining1 { get; set; }
    }

    public class ValueBasedWithTwoProp : EntityBase
    {
        [UsedInEntityByValueComparison]
        public int ValueDefining1 { get; set; }

        [UsedInEntityByValueComparison]
        public int ValueDefining2 { get; set; }
    }

    public class ValueBasedWithOnePropAndOneExtra : EntityBase
    {
        public ValueBasedWithOnePropAndOneExtra()
        {
        }

        public ValueBasedWithOnePropAndOneExtra(int valueDefining1, int notValueDefining1)
        {
            ValueDefining1 = valueDefining1;
            NotValueDefining1 = notValueDefining1;
        }

        [UsedInEntityByValueComparison]
        public int ValueDefining1 { get; set; }
        
        // not used in value comparison!
        public int NotValueDefining1 { get; set; }
    }

    public class ValueBasedWithNavProp : EntityBase
    {
        public ValueBasedWithNavProp()
        {
            
        }

        public ValueBasedWithNavProp(DummyEnt dummyProp)
        {
            DummyNav = dummyProp;
        }

        [UsedInEntityByValueComparison]
        public DummyEnt DummyNav { get; set; }
    }

    public class ValueBasedWithOwnedPropWithConversion : EntityBase
    {
        private ValueBasedWithOwnedPropWithConversion()
        {
        }

        public ValueBasedWithOwnedPropWithConversion(SimpleRecord withConvert)
        {
            WithConvert = withConvert;
        }

        [UsedInEntityByValueComparison]
        public SimpleRecord WithConvert { get; private set; }
    }


    public class ValueBasedWithOwnedPropWithConversion_NonTrivialEquals : EntityBase
    {
        private ValueBasedWithOwnedPropWithConversion_NonTrivialEquals()
        {
            
        }

        public ValueBasedWithOwnedPropWithConversion_NonTrivialEquals(NonTrivialEqualityRecord withConvert)
        {
            WithConvert = withConvert;
        }

        [UsedInEntityByValueComparison]
        public NonTrivialEqualityRecord WithConvert { get; private set; }
    }

    public record NonTrivialEqualityRecord(string Value)
    {
        public virtual bool Equals(NonTrivialEqualityRecord? other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other is null) return false;

            return EqualityMap[Value].Equals(EqualityMap[other.Value]);
        }

        public override int GetHashCode()
        {
            return EqualityMap[Value].GetHashCode();
        }

        public static readonly IImmutableDictionary<string, string> EqualityMap = new Dictionary<string,string>()
        {
            {"One", "1"},
            {"Two", "2"},
            {"Three", "3"},
            {"1", "1"},
            {"2", "2"},
            {"3", "3"}
        }.ToImmutableDictionary();


        public class StringConverter() : ValueConverter<NonTrivialEqualityRecord, string>
        (
            x => EqualityMap[x.Value],
            x => new NonTrivialEqualityRecord(x)
        );
    };

    public record SimpleRecord(string Value)
    {
        public class StringConverter() : ValueConverter<SimpleRecord, string>
        (
            x => x.Value,
            x => new SimpleRecord(x)
        );
    };

    public class DummyEnt : EntityBase
    {
        public DummyEnt()
        {
        }
    }
}