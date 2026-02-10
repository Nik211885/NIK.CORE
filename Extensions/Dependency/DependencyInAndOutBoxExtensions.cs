using Microsoft.Extensions.DependencyInjection;
using NIK.CORE.DOMAIN.Abstracts;
using NIK.CORE.DOMAIN.Inbox;
using NIK.CORE.DOMAIN.Outbox;

namespace NIK.CORE.DOMAIN.Extensions.Dependency;

public static class DependencyInAndOutBoxExtensions
{
    extension(IServiceCollection serviceCollection)
    {
        public IServiceCollection AddInOutBox<TDbContext>() where TDbContext : BaseDbContext
        {
            serviceCollection.AddInbox<TDbContext>();
            serviceCollection.AddOutbox<TDbContext>();
            return serviceCollection;
        }

        public IServiceCollection AddInbox<TDbContext>() where TDbContext : BaseDbContext
        {
            serviceCollection.AddScoped<IOutboxStore, OutboxStore>(sp =>
            {
                var dbContext = sp.GetRequiredService<TDbContext>();
                return new OutboxStore(dbContext);
            });
            serviceCollection.AddScoped<InboxCleanupJob>();
            return serviceCollection;
        }

        public IServiceCollection AddOutbox<TDbContext>() where TDbContext : BaseDbContext
        {
            serviceCollection.AddScoped<IInboxStore, InboxStore>(sp =>
            {
                var dbContext = sp.GetRequiredService<TDbContext>();
                return new InboxStore(dbContext);
            });
            serviceCollection.AddScoped<OutboxProcessor>();
            serviceCollection.AddScoped<OutboxCleanupJob>();
            return serviceCollection;
        }
    }
}
