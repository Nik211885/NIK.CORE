using System.Diagnostics.CodeAnalysis;

namespace NIK.CORE.DOMAIN.Exceptions;

/// <summary>
/// A centralized helper class to provide a consistent and performant way to throw domain-specific exceptions.
/// </summary>
/// <remarks>
/// Using a ThrowHelper is a best practice in .NET as it improves code readability and 
/// can help with "Hot Path" optimization by reducing the code size of calling methods (inlining).
/// </remarks>
public static class ThrowHelper
{
    /// <summary> Throws a <see cref="BadRequestException"/> (HTTP 400). </summary>
    [DoesNotReturn]
    public static void ThrowBadRequest(string message = CoreMessages.BadRequest)
    {
        throw new BadRequestException(message);
    }

    /// <summary> Throws a <see cref="PermissionDeniedException"/> (HTTP 403). </summary>
    [DoesNotReturn]
    public static void ThrowPermissionDenied(string message = CoreMessages.PermissionDenied)
    {
        throw new PermissionDeniedException(message);
    }

    /// <summary> Throws a <see cref="ConflictException"/> (HTTP 409). </summary>
    [DoesNotReturn]
    public static void ThrowConflict(string message = CoreMessages.Conflict)
    {
        throw new ConflictException(message);
    }

    /// <summary> Throws a <see cref="NotFoundException"/> (HTTP 404). </summary>
    [DoesNotReturn]
    public static void ThrowNotFound(string message = CoreMessages.NotFound)
    {
        throw new NotFoundException(message);
    }

    /// <summary> Throws an <see cref="UnAuthorizedException"/> (HTTP 401). </summary>
    [DoesNotReturn]
    public static void ThrowUnauthorized(string message = CoreMessages.Unauthorized)
    {
        throw new UnAuthorizedException(message);
    }
}
