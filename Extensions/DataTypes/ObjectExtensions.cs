using System.Collections.Concurrent;
using System.Reflection;

namespace NIK.CORE.DOMAIN.Extensions.DataTypes;

public static class ObjectExtensions
{
    private static readonly ConcurrentDictionary<(Type, Type), PropertyMap[]> Cache = new();

    extension(object obj)
    {
        /// <summary>
        /// Creates a new <typeparamref name="T"/> and maps matching readable/writable
        /// properties from the source object using reflection.
        /// </summary>
        /// <typeparam name="T">Target type with parameterless constructor.</typeparam>
        /// <returns>Mapped instance of <typeparamref name="T"/>.</returns>
        T ReflectionMapTo<T>() where T : new()
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
    }

    private static PropertyMap[] BuildPropertyMap(Type sourceType, Type targetType)
    {
        var sourceProps = sourceType.GetProperties()
            .Where(p => p.CanRead)
            .ToDictionary(p => p.Name);
        return targetType.GetProperties()
            .Where(p => p.CanWrite)
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
