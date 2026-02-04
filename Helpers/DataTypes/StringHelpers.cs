using System.Security.Cryptography;
using System.Text;

namespace NIK.CORE.DOMAIN.Helpers.DataTypes;
/// <summary>
///  
/// </summary>
public static class StringHelpers
{
    /// <summary>
    ///     Allowed characters used for generating random slug prefix.
    /// </summary>
    private static readonly char[] SlugChars = "1234567890qwertyuiopasdfghjklzxcvbnm".ToCharArray();
    private static readonly Random Random = new();
    /// <summary>
    ///     Generates a cryptographically secure random Base64 string.
    /// </summary>
    /// <param name="bytes">Number of random bytes to generate.</param>
    /// <returns>A Base64-encoded random string.</returns>
    public static string GeneratorRandomStringBase64(int bytes)
    {
        var rand = new byte[bytes];
        using var random = RandomNumberGenerator.Create();
        random.GetBytes(rand);
        return Convert.ToBase64String(rand);
    }
    /// <summary>
    ///     Generates a random alphanumeric string.
    /// </summary>
    /// <param name="length">Length of the generated string.</param>
    /// <returns>Random string.</returns>
    public static string GenerateRandomPart(int length)
    {
        var sb = new StringBuilder(length);
        for (int i = 0; i < length; i++)
        {
            sb.Append(SlugChars[Random.Next(SlugChars.Length)]);
        }
        return sb.ToString();
    }
}
