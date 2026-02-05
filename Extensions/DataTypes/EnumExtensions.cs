using System.ComponentModel;
using System.Reflection;

namespace NIK.CORE.DOMAIN.Extensions.DataTypes;
/// <summary>
/// 
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Provides extension methods for reading metadata from an enum type.
    /// </summary>
    /// <param name="enum">The enum value.</param>
    extension(Enum @enum)
    {
        /// <summary>
        /// Gets the <see cref="DescriptionAttribute"/> applied to the enum type.
        /// </summary>
        /// <returns>
        /// The description defined on the enum type, or an empty string if not found.
        /// </returns>
        string GetDescription()
        {
            var attributeDescription =
                @enum.GetType().GetCustomAttribute<DescriptionAttribute>();
            if (attributeDescription is null)
                return string.Empty;
            return attributeDescription.Description;
        }
    }

}
