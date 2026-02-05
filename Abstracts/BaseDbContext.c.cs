using Microsoft.EntityFrameworkCore;
using NIK.CORE.DOMAIN.Helpers;

namespace NIK.CORE.DOMAIN.Abstracts;
/// <summary>
/// Base <see cref="DbContext"/> that defines common EF Core conventions
/// shared across all application DbContexts.
/// </summary>
/// <remarks>
/// This base context configures a global value conversion for all properties
/// derived from <see cref="TypedIdValueBase"/>.
/// <para>
/// Any concrete <see cref="DbContext"/> inheriting from this class will
/// automatically persist TypedId value objects as <see cref="Guid"/> values
/// in the database, without requiring per-property configuration.
/// </para>
/// <para>
/// This approach ensures:
/// <list type="bullet">
///   <item>Clean domain models with strongly-typed IDs</item>
///   <item>No repetitive <c>HasConversion</c> mappings</item>
///   <item>Consistent mapping behavior for primary keys and foreign keys</item>
/// </list>
/// </para>
/// </remarks>
public abstract class BaseDbContext : DbContext
{
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
    /// Configures model-wide conventions applied during model creation.
    /// </summary>
    /// <param name="configurationBuilder">
    /// The builder used to define conventions that apply to all entity types
    /// in the model.
    /// </param>
    /// <remarks>
    /// This convention automatically applies <see cref="TypedIdValueBase"/>
    /// to all properties whose CLR type derives from <see cref="TypedIdValueConverter{TTypedIdValue}"/>.
    /// </remarks>
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<TypedIdValueBase>()
            .HaveConversion(typeof(TypedIdValueConverter<>));
    }
}
