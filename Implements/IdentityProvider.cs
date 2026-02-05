using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using NIK.CORE.DOMAIN.Contracts;

namespace NIK.CORE.DOMAIN.Implements;

/// <summary>
///     Default implementation of <see cref="IIdentityProvider"/>
///     that retrieves identity information from the current HTTP context.
/// </summary>
public class IdentityProvider : IIdentityProvider
{
    private readonly IHttpContextAccessor _contextAccessor;

    public IdentityProvider(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    private ClaimsPrincipal? User => _contextAccessor.HttpContext?.User;

    /// <summary>
    ///     Indicates whether the current user is authenticated.
    /// </summary>
    public bool IsAuthenticated
        => User?.Identity?.IsAuthenticated ?? false;

    /// <summary>
    ///     Gets the unique identifier of the current user.
    /// </summary>
    public string UserId
        => User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

    /// <summary>
    ///     Gets the username of the current user.
    /// </summary>
    public string UserName
        => User?.Identity?.Name ?? string.Empty;

    /// <summary>
    ///     Determines whether the current user belongs to the specified role.
    /// </summary>
    /// <param name="role">The role name.</param>
    /// <returns>
    ///     <c>true</c> if the user is in the specified role; otherwise <c>false</c>.
    /// </returns>
    public bool IsInRole(string role)
        => User?.IsInRole(role) ?? false;
    /// <summary>
    ///  Get Ip address from request
    /// </summary>
    public string IpAddress 
        => _contextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

    /// <summary>
    ///     Determines whether the current user has the specified permission.
    ///     <para>
    ///     Permissions are represented as claims.
    ///     </para>
    /// </summary>
    /// <param name="permission">The permission value.</param>
    /// <returns>
    ///     <c>true</c> if the user has the permission; otherwise <c>false</c>.
    /// </returns>
    public bool IsInPermission(string permission)
        => User?.Claims.Any(c => c.Type == "permission" && c.Value == permission) ?? false;

    /// <summary>
    ///     Determines whether the current user has a specific claim.
    /// </summary>
    /// <param name="type">The claim type.</param>
    /// <param name="value">The claim value.</param>
    /// <returns>
    ///     <c>true</c> if the claim exists; otherwise <c>false</c>.
    /// </returns>
    public bool HasClaim(string type, string value)
        => User?.HasClaim(type, value) ?? false;

    /// <summary>
    ///     Gets all roles assigned to the current user.
    /// </summary>
    /// <returns>
    ///     A read-only collection of role names.
    /// </returns>
    public IReadOnlyCollection<string> GetRoles()
        => User?.Claims.Where(c => c.Type == ClaimTypes.Role) .Select(c => c.Value).Distinct()
            .ToList() ?? [];

    /// <summary>
    ///     Gets all permissions assigned to the current user.
    /// </summary>
    /// <returns>
    ///     A read-only collection of permissions.
    /// </returns>
    public IReadOnlyCollection<string> GetPermissions()
        => User?.Claims .Where(c => c.Type == "permission").Select(c => c.Value)
            .Distinct().ToList() ?? [];

    /// <summary>
    ///     Gets all claims of the current user.
    /// </summary>
    /// <returns>
    ///     A read-only collection of <see cref="Claim"/>.
    /// </returns>
    public IReadOnlyCollection<Claim> GetClaims()
        => User?.Claims.ToList() ?? [];

    /// <summary>
    ///     Gets all claims of the specified type for the current user.
    /// </summary>
    /// <param name="claimType">The claim type.</param>
    /// <returns>
    ///     A read-only collection of matching <see cref="Claim"/>.
    /// </returns>
    public IReadOnlyCollection<Claim> GetClaims(string claimType)
        => User?.Claims .Where(c => c.Type == claimType) .ToList() ?? [];
}
