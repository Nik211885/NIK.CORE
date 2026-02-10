using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using NIK.CORE.DOMAIN.Attributes;
using NIK.CORE.DOMAIN.Extensions.EntityFramework.Models;

namespace NIK.CORE.DOMAIN.Extensions.EntityFramework;

/// <summary>
/// Provides LINQ-to-Entities friendly projection helpers for <see cref="IQueryable"/>.
/// Expressions are built once and cached for reuse to improve performance.
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Caches mapping expressions by source and target type.
    /// </summary>
    private static readonly ConcurrentDictionary<(Type, Type), LambdaExpression> Cache = new();

    extension<TE>(IQueryable<TE> queryable)
    {
        /// <summary>
        /// Projects the current query to <typeparamref name="T"/> by dynamically
        /// building and caching a mapping expression based on matching property names.
        /// </summary>
        /// <typeparam name="T">
        /// Target type with a parameterless constructor.
        /// </typeparam>
        /// <returns>
        /// An <see cref="IQueryable{T}"/> that can be translated by LINQ providers
        /// such as Entity Framework.
        /// </returns>
        public IQueryable<T> MapTo<T>() where T : new()
        {
            var key = (typeof(TE), typeof(T));
            if (!Cache.TryGetValue(key, out var cached))
            {
                cached = BuildExpression<TE, T>();
                Cache[key] = cached;
            }
            return queryable.Select((Expression<Func<TE, T>>)cached);
        }
        /// <summary>
        /// Executes a paginated query and returns a paginated result set.
        /// </summary>
        /// <typeparam name="T">
        /// The destination type of the projected items.
        /// </typeparam>
        /// <param name="pageRequest">
        /// The pagination parameters including page number and page size.
        /// </param>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// A <see cref="PaginationItem{T}"/> containing the paged items and pagination metadata.
        /// </returns>
        /// <remarks>
        /// This method normalizes invalid pagination parameters by applying default values,
        /// executes the query using skip/take semantics, and maps the source entities to the
        /// specified destination type.
        /// </remarks>
        public async Task<PaginationItem<T>> PaginationAsync<T>(PaginationRequest pageRequest, CancellationToken cancellationToken = default) where T : new()
        {
            int pageNumber = pageRequest.PageNumber <= 0 ? 1 : pageRequest.PageNumber;
            int pageSize = pageRequest.PageSize <= 0 ? 10 : pageRequest.PageSize;
            int totalCount = await queryable.CountAsync(cancellationToken);
            
            var items = await queryable
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .MapTo<TE, T>()
                .ToListAsync(cancellationToken);
            return new PaginationItem<T>(items, pageNumber, pageSize, totalCount);
        }
    }

    /// <summary>
    /// Builds a projection expression that maps readable source properties
    /// to writable target properties with the same name and compatible types.
    /// </summary>
    private static Expression<Func<TSource, TTarget>> BuildExpression<TSource, TTarget>()
        where TTarget : new()
    {
        var sourceType = typeof(TSource);
        var targetType = typeof(TTarget);
        // Represents the source element in the query (x => ...)
        var parameter = Expression.Parameter(sourceType, "x");
        var bindings = new List<MemberBinding>();
        foreach (var targetProp in targetType.GetProperties())
        {
            if (!targetProp.CanWrite)
                continue;
            var sourceProp = sourceType.GetProperty(targetProp.Name);
            if (sourceProp is null || !sourceProp.CanRead || sourceProp.GetCustomAttribute<IgnoreMappingAttribute>() is null)
                continue;
            if (!targetProp.PropertyType.IsAssignableFrom(sourceProp.PropertyType))
                continue;
            bindings.Add(
                Expression.Bind(
                    targetProp,
                    Expression.Property(parameter, sourceProp)
                )
            );
        }
        // Builds: x => new TTarget { Prop = x.Prop, ... }
        var body = Expression.MemberInit(Expression.New(targetType), bindings);
        return Expression.Lambda<Func<TSource, TTarget>>(body, parameter);
    }
}
