using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Reflection;

namespace EntityTools.DbContextBackedService;

/// <inheritdoc />
public abstract class DbContextBackedService<T>(T db, ILogger? logger = null)
    : DbContextBackedService(db, logger)
    where T : DbContext
{
    protected readonly T Db = db;
    private readonly ILogger _logger = logger ?? NullLogger.Instance;
}


/// <summary>
/// Contains core methods for Services backed by a DbContext. Typically you'll want to use <see cref="DbContextBackedService{T}"/>
/// </summary>
public abstract class DbContextBackedService(DbContext db, ILogger? logger = null)
{
    private readonly DbContext _backingDbContext = db;
    private readonly ILogger _logger = logger ?? NullLogger.Instance;

    /// <summary>
    /// Public reference is an ID intended to be used external to the db (as opposed to the internal implementation-dependent ID like a sequential PK).
    /// Public is most often a GUID, PID, custom defined ID, or similar. It is generally persistent and not dependent on the database / storage format.
    /// 
    /// Technically, this can be used for internal ID too atm?
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TReference"></typeparam>
    /// <param name="refs"></param>
    /// <param name="publicReferenceProperty"></param>
    /// <param name="maxComparisonsPerQuery"></param>
    /// <param name="includeNavProps"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    protected async Task<IReadOnlyCollection<TEntity>> FetchPersistedEntitiesByPublicReference<TEntity, TReference>(
        IEnumerable<TReference> refs,
        PropertyInfo publicReferenceProperty,
        int maxComparisonsPerQuery = 10_000,
        IEnumerable<PropertyInfo>? includeNavProps = null,
        CancellationToken token = default)
        where TEntity : class
        where TReference : struct, IEquatable<TReference>
    {
        if (publicReferenceProperty.PropertyType != typeof(TReference))
            throw new ArgumentException(
                $"Type mismatch between the provided property {publicReferenceProperty.Name} ({publicReferenceProperty.PropertyType}) and the public references ({typeof(TReference)})");

        var uniqueRefs = refs.Distinct().ToList();

        var res = await EntityHelper.FetchEntitiesByValue
        (
            _backingDbContext.Set<TEntity>().AsQueryable(),
            uniqueRefs,
            publicReferenceProperty,
            maxComparisonsPerQuery,
            includeNavProps?.Distinct().ToList(),
            maxComparisonsPerQuery,
            token
        );

        return res.Existing;
    }

    /// <summary>
    /// From the provided "dummy" entities, see which ones are already persisted in the db.
    /// "Dummy" entities are constructed like a real entity would be, but setting only value-carrying properties, not "id" properties like the primary key or similar.
    /// Dummy entities are not referentially equal to their real counterparts and have a meaningless ID.
    /// Returned existing entities will be proper, tracked, referentially-comparable entities with correct IDs. Missing ones will be the same dummy instances as in the query.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dummyEntities"></param>
    /// <param name="maxComparisonsPerQuery"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    protected Task<(IReadOnlyCollection<T> Existing, IReadOnlyCollection<T> Missing)> GetExistingAndMissingEntitiesByValue<T>(
        IEnumerable<T> dummyEntities,
        int maxComparisonsPerQuery = 10_000,
        CancellationToken token = default) where T : class
    {
        return EntityHelper.GetExistingAndMissingEntitiesByValue
        (
            _backingDbContext,
            dummyEntities,
            maxComparisonsPerQuery,
            token
        );
    }

    /// <summary>
    /// From the provided "dummy" entities, get the persisted ones from the db. Optionally, creates new ones if not present in the db.
    /// "Dummy" entities are constructed like a real entity would be, but setting only value-carrying properties, not "id" properties like the primary key or similar.
    /// Dummy entities are not referentially equal to their real counterparts and have a meaningless ID.
    /// Returned items will have a real ID from the DB and be referentially equal and tracked as normal.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dummyEntities"></param>
    /// <param name="createNewIfMissing"></param>
    /// <param name="eagerPersist"></param>
    /// <param name="maxComparisonsPerQuery"></param>
    /// <param name="onNewCreatedCallback"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    protected async Task<IReadOnlyCollection<T>> GetPersistedValueLikeEntities<T>(
        IEnumerable<T> dummyEntities,
        bool createNewIfMissing, 
        bool eagerPersist, 
        int maxComparisonsPerQuery = 10_000,
        Action<IReadOnlyCollection<T>>? onNewCreatedCallback = null, 
        CancellationToken token = default) where T : class
    {
        var (existing, missing) = await GetExistingAndMissingEntitiesByValue
        (
            dummyEntities,
            maxComparisonsPerQuery,
            token
        );

        if (missing.Count > 0)
        {
            if (!createNewIfMissing)
            {
                await _backingDbContext.AddRangeAsync(missing, token);
                if (eagerPersist) await _backingDbContext.SaveChangesAsync(token);
                onNewCreatedCallback?.Invoke(missing);
            }
            else
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                    _logger.Log(LogLevel.Debug, "Will not create {} missing items, as creation of new items is disallowed by input", missing.Count);
            }
        }

        var res = existing.Concat(missing).ToList();
        return res;
    }

    /// <summary>
    /// From the provided items, get matching persisted entities from the db. Entities created using provided instanceCreator.
    /// Optionally, creates new entities if not present in the db. If not allowed, the number of outputs could be less than # of inputs.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TInput"></typeparam>
    /// <param name="items"></param>
    /// <param name="instanceCreator"></param>
    /// <param name="allowCreateNew"></param>
    /// <param name="eagerPersist"></param>
    /// <param name="maxComparisonsPerQuery"></param>
    /// <param name="onNewCreatedCallback"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    protected Task<IReadOnlyCollection<TEntity>> GetPersistedValueLikeEntities<TEntity, TInput>(
        IEnumerable<TInput> items,
        Func<TInput, TEntity> instanceCreator,
        bool allowCreateNew, 
        bool eagerPersist,
        int maxComparisonsPerQuery = 10_000,
        Action<IReadOnlyCollection<TEntity>>? onNewCreatedCallback = null, 
        CancellationToken token = default
        )
        where TEntity : class
        where TInput : IEquatable<TInput>
    {
        var unique = items.Distinct().ToList();
        var dummyEnts = unique.Select(instanceCreator).ToList();
        var persisted = GetPersistedValueLikeEntities(dummyEnts, allowCreateNew, eagerPersist, maxComparisonsPerQuery, onNewCreatedCallback, token);
        return persisted;
    }
}