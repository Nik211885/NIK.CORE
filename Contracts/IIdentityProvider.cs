using System.Security.Claims;

namespace NIK.CORE.DOMAIN.Contracts;

/// <summary>
///     Provides access to information about the current authenticated user
///     and their identity, roles, permissions, and claims.
/// </summary>
public interface IIdentityProvider
{
    /// <summary>
    ///     Indicates whether the current user is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    ///     Gets the unique identifier of the current user.
    /// </summary>
    string UserId { get; }

    /// <summary>
    ///     Gets the username of the current user.
    /// </summary>
    string UserName { get; }

    /// <summary>
    ///     Determines whether the current user belongs to the specified role.
    /// </summary>
    /// <param name="role">
    ///     The role name to check.
    /// </param>
    /// <returns>
    ///     True if the user is in the specified role; otherwise, false.
    /// </returns>
    bool IsInRole(string role);

    /// <summary>
    ///     Determines whether the current user has the specified permission.
    /// </summary>
    /// <param name="permission">
    ///     The permission name to check.
    /// </param>
    /// <returns>
    ///     True if the user has the specified permission; otherwise, false.
    /// </returns>
    bool IsInPermission(string permission);
    /// <summary>
    ///     Determines whether the current user has a claim
    ///     with the specified type and value.
    /// </summary>
    /// <param name="type">
    ///     The claim type to check.
    /// </param>
    /// <param name="value">
    ///     The claim value to check.
    /// </param>
    /// <returns>
    ///     True if the user has the specified claim; otherwise, false.
    /// </returns>
    bool HasClaim(string type, string value);

    /// <summary>
    ///     Gets all roles associated with the current user.
    /// </summary>
    /// <returns>
    ///     A read-only collection of role names.
    /// </returns>
    IReadOnlyCollection<string> GetRoles();

    /// <summary>
    ///     Gets all permissions granted to the current user.
    /// </summary>
    /// <returns>
    ///     A read-only collection of permission names.
    /// </returns>
    IReadOnlyCollection<string> GetPermissions();

    /// <summary>
    ///     Gets all claims associated with the current user.
    /// </summary>
    /// <returns>
    ///     A read-only collection of claims.
    /// </returns>
    IReadOnlyCollection<Claim> GetClaims();

    /// <summary>
    ///     Gets all claims of the specified type associated with the current user.
    /// </summary>
    /// <param name="claimType">
    ///     The type of claim to retrieve.
    /// </param>
    /// <returns>
    ///     A read-only collection of claims matching the specified type.
    /// </returns>
    IReadOnlyCollection<Claim> GetClaims(string claimType);
}