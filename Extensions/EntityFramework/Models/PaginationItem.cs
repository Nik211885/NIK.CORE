namespace NIK.CORE.DOMAIN.Extensions.EntityFramework.Models;
/// <summary>
/// Represents a paginated result set.
/// </summary>
/// <typeparam name="TEntity">
/// The type of items contained in the paginated result.
/// </typeparam>
/// <remarks>
/// This model encapsulates pagination metadata along with the current page
/// of items, and is commonly used for returning paged data from queries or APIs.
/// </remarks>
public class PaginationItem<TEntity>(
    IReadOnlyCollection<TEntity> items,
    int pageNumber,
    int pageSize,
    int count)
{
    /// <summary>
    /// Gets the items contained in the current page.
    /// </summary>
    public IReadOnlyCollection<TEntity> Items { get; } = items;

    /// <summary>
    /// Gets the current page number (1-based).
    /// </summary>
    public int PageNumber { get; } = pageNumber;

    /// <summary>
    /// Gets the total number of pages.
    /// </summary>
    public int TotalPages { get; } =
        (int)Math.Ceiling(count / (double)pageSize);

    /// <summary>
    /// Gets the total number of items across all pages.
    /// </summary>
    public int TotalCount { get; } = count;

    /// <summary>
    /// Gets the number of items per page.
    /// </summary>
    public int PageSize { get; } = pageSize;

    /// <summary>
    /// Gets a value indicating whether a previous page exists.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Gets a value indicating whether a next page exists.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;
}

/// <summary>
/// Represents pagination parameters for a query request.
/// </summary>
/// <param name="PageNumber">
/// The requested page number (1-based).
/// </param>
/// <param name="PageSize">
/// The number of items to include per page.
/// </param>
public record PaginationRequest(
    int PageNumber,
    int PageSize);
