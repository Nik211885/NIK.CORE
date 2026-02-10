using System.Collections.Concurrent;
using System.Reflection;
using NIK.CORE.DOMAIN.Attributes;

namespace NIK.CORE.DOMAIN.Extensions.DataTypes;

public static class ObjectExtensions
{
    private static readonly ConcurrentDictionary<(Type, Type), PropertyMap[]> Cache = new();

    extension(object? obj)
    {
        /// <summary>
        /// Creates a new <typeparamref name="T"/> and maps matching readable/writable
        /// properties from the source object using reflection.
        /// </summary>
        /// <typeparam name="T">Target type with parameterless constructor.</typeparam>
        /// <returns>Mapped instance of <typeparamref name="T"/>.</returns>
        public T ReflectionMapTo<T>() where T : new()
        {
            ArgumentNullException.ThrowIfNull(obj);
            var sourceType = obj.GetType();
            var targetType = typeof(T);
            var maps = Cache.GetOrAdd(
                (sourceType, targetType),
                _ => BuildPropertyMap(sourceType, targetType)
            );
            var target = new T();
            foreach (var map in maps)
            {
                var value = map.Source.GetValue(obj);
                if (value is null)
                    continue;
                map.Target.SetValue(target, value);
            }
            return target;
        }
        /// <summary>
        /// Determines whether the object is null.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the object is null; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNull() => obj is null;
        /// <summary>
        /// Determines whether the object is not null.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the object is not null; otherwise, <c>false</c>.
        /// </returns>
        public bool NotNull() => obj is not null;
        /// <summary>
        /// Returns the object cast to <typeparamref name="T"/> if possible;
        /// otherwise returns the specified default value.
        /// </summary>
        /// <typeparam name="T">Expected type.</typeparam>
        /// <param name="defaultValue">Value to return if cast fails.</param>
        /// <returns>
        /// The cast value or <paramref name="defaultValue"/>.
        /// </returns>
        public T GetOrDefault<T>(T defaultValue = default!)
            => obj is T t ? t : defaultValue;
        /// <summary>
        /// Determines whether the object is of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type to check against.</typeparam>
        /// <returns>
        /// <c>true</c> if the object is of type <typeparamref name="T"/>; otherwise, <c>false</c>.
        /// </returns>
        public bool Is<T>() => obj is T;
        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> if the object is null.
        /// </summary>
        /// <param name="name">
        /// The name of the parameter to include in the exception.
        /// </param>
        public void ThrowIfNull(string? name = null)
            => ArgumentNullException.ThrowIfNull(obj, name);
        /// <summary>
        /// Determines whether the object has a meaningful value.
        /// </summary>
        /// <remarks>
        /// Returns <c>false</c> for <c>null</c> and empty or whitespace strings.
        /// </remarks>
        /// <returns>
        /// <c>true</c> if the object has a value; otherwise, <c>false</c>.
        /// </returns>
        public bool HasValue()
            => obj switch
            {
                null => false,
                string s => !string.IsNullOrWhiteSpace(s),
                _ => true
            };
    }
    private static PropertyMap[] BuildPropertyMap(Type sourceType, Type targetType)
    {
        var sourceProps = sourceType.GetProperties()
            .Where(p => p.CanRead)
            .ToDictionary(p => p.Name);
        return targetType.GetProperties()
            .Where(p => p.CanWrite && p.GetCustomAttribute<IgnoreMappingAttribute>() is not null)
            .Select(tp =>
            {
                if (!sourceProps.TryGetValue(tp.Name, out var sp))
                    return null;
                if (!tp.PropertyType.IsAssignableFrom(sp.PropertyType))
                    return null;
                return new PropertyMap(sp, tp);
            })
            .Where(m => m is not null)
            .ToArray()!;
    }
    private sealed record PropertyMap(
        PropertyInfo Source,
        PropertyInfo Target
    );
}
