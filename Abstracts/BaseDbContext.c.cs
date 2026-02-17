using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NIK.CORE.DOMAIN.Configs;
using NIK.CORE.DOMAIN.Contracts;
using NIK.CORE.DOMAIN.Helpers;
using NIK.CORE.DOMAIN.Inbox;
using NIK.CORE.DOMAIN.Outbox;

namespace NIK.CORE.DOMAIN.Abstracts;

/// <summary>
/// Base <see cref="DbContext"/> implementation that provides common
/// EF Core conventions and unit-of-work capabilities.
/// </summary>
/// <remarks>
/// This base context centralizes:
/// <list type="bullet">
///   <item>Typed ID value object conversion</item>
///   <item>Transaction management</item>
///   <item>Execution strategy support for resiliency</item>
/// </list>
/// </remarks>
public abstract class BaseDbContext : DbContext, IUnitOfWork
{
    private IDbContextTransaction? _currentTransaction;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseDbContext"/> class.
    /// </summary>
    /// <param name="options">
    /// The options to be used by the <see cref="DbContext"/>.
    /// </param>
    protected BaseDbContext(DbContextOptions options)
        : base(options)
    {
    }
    /// <summary>
    /// Represents the Inbox message store used to ensure idempotent
    /// processing of incoming integration events.
    /// </summary>
    /// <remarks>
    /// This table is used by the Inbox pattern to track messages received
    /// from external systems or message brokers.
    /// <para>
    /// It helps prevent duplicate processing when the same message
    /// is delivered more than once.
    /// </para>
    /// </remarks>
    public DbSet<InboxMessage> InboxMessages { get; set; }

    /// <summary>
    /// Represents the Outbox message store used to reliably publish
    /// integration events.
    /// </summary>
    /// <remarks>
    /// This table is used by the Outbox pattern to persist integration events
    /// within the same database transaction as business data changes.
    /// <para>
    /// Messages stored here are later published to the message broker
    /// by a background processor, ensuring at-least-once delivery.
    /// </para>
    /// </remarks>
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    /// <summary>
    /// Gets or sets the DbSet for configuration keys.
    /// </summary>
    /// <remarks>
    /// Represents the collection of configuration key entities stored in the database.
    /// Configuration keys define the available configuration entries.
    /// </remarks>
    public DbSet<ConfigKey> ConfigKeys { get; set; }

    /// <summary>
    /// Gets or sets the DbSet for configuration values.
    /// </summary>
    /// <remarks>
    /// Represents the collection of configuration value entities stored in the database.
    /// Configuration values store the actual values associated with configuration keys,
    /// potentially varying by environment, tenant, or scope.
    /// </remarks>
    public DbSet<ConfigValue> ConfigValues { get; set; }
    /// <summary>
    /// Configures model-wide conventions applied during model creation.
    /// </summary>
    /// <param name="configurationBuilder">
    /// The builder used to define conventions that apply to all entity types
    /// in the model.
    /// </param>
    /// <remarks>
    /// Automatically applies a value converter for all properties
    /// derived from <see cref="TypedIdValueBase"/>, ensuring they are
    /// persisted as <see cref="Guid"/> values in the database.
    /// </remarks>
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<TypedIdValueBase>()
            .HaveConversion(typeof(TypedIdValueConverter<>));
    }

    /// <summary>
    /// Applies entity configurations from the current assembly.
    /// </summary>
    /// <param name="modelBuilder">
    /// The builder used to construct the entity model.
    /// </param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Begins a new database transaction.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token to observe while waiting for the operation to complete.
    /// </param>
    public async Task BeginTransactionAsync(CancellationToken cancellationToken)
        => _currentTransaction = await base.Database.BeginTransactionAsync(cancellationToken);

    /// <summary>
    /// Persists all tracked changes to the database.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token to observe while waiting for the operation to complete.
    /// </param>
    /// <returns>
    /// The number of state entries written to the database.
    /// </returns>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await base.SaveChangesAsync(cancellationToken);

    /// <summary>
    /// Commits the current transaction and persists all pending changes.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token to observe while waiting for the operation to complete.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when no active transaction exists.
    /// </exception>
    public async Task CommitTransactionAsync(CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(_currentTransaction);
        try
        {
            await SaveChangesAsync(cancellationToken);
            await _currentTransaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await _currentTransaction.CommitAsync(cancellationToken);
            throw;
        }
        finally
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    /// <summary>
    /// Rolls back the current transaction.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token to observe while waiting for the operation to complete.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when no active transaction exists.
    /// </exception>
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(_currentTransaction);
        await _currentTransaction.RollbackAsync(cancellationToken);
        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
    }

    /// <summary>
    /// Gets the identifier of the currently active database transaction.
    /// </summary>
    /// <remarks>
    /// Returns <see cref="Guid.Empty"/> when no transaction is active.
    /// </remarks>
    public Guid CurrentTransactionId
        => _currentTransaction?.TransactionId ?? Guid.Empty;

    /// <summary>
    /// Executes the specified action using the configured EF Core execution strategy.
    /// </summary>
    /// <remarks>
    /// This method enables resiliency features such as automatic retries
    /// for transient failures (e.g., network or database timeouts),
    /// depending on the configured database provider.
    /// </remarks>
    /// <param name="action">
    /// The asynchronous operation to execute.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous execution of the operation.
    /// </returns>
    public async Task ExecutionStrategyAsync(Func<Task> action)
    {
        var strategy = Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await action();
        });
    }
}
