namespace NIK.CORE.DOMAIN.Contracts;
/// <summary>
///     Represents a repository for an aggregate root.
///     The repository is responsible for managing persistence
///     and ensuring consistency boundaries of the aggregate.
///     It also exposes a unit of work to coordinate transactional operations.
/// </summary>
/// <typeparam name="T">
///     Type of the aggregate root.
/// </typeparam>
public interface IRepository<T> where T : IAggregateRoot
{
    /// <summary>
    ///     Gets the unit of work associated with the repository.
    /// </summary>
    IUnitOfWork UnitOfWork { get; }
}