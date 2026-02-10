using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using NIK.CORE.DOMAIN.Helpers.DataTypes;

namespace NIK.CORE.DOMAIN.Extensions.DataTypes;
/// <summary>
/// 
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    extension(string value)
    {
        /// <summary>
        ///     Generates a slug in the format: randomPart/normalized-text.
        /// </summary>
        /// <returns>A URL-safe slug string.</returns>
        public string GeneratorSlug()
        {
            var prefix = StringHelpers.GenerateRandomPart(6);
            var suffix = value.GenerateSlugPart();
            return $"{prefix}/{suffix}";
        }
        /// <summary>
        ///     Converts input text into a URL-friendly slug.
        /// </summary>
        /// <returns>Normalized slug text.</returns>
        public string GenerateSlugPart()
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;
            var normalized = value.Normalize(NormalizationForm.FormD);
            var withoutDiacritics = new string(
                normalized .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark) .ToArray()
            );
            var slug = withoutDiacritics
                .Normalize(NormalizationForm.FormC)
                .ToLowerInvariant();
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
            slug = Regex.Replace(slug, @"\s+", "-").Trim('-');
            return slug;
        }
    }
}
