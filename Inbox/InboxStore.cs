using Microsoft.Extensions.Logging;
using NIK.CORE.DOMAIN.Abstracts;

namespace NIK.CORE.DOMAIN.Inbox;

public sealed class InboxStore : IInboxStore
{
    private readonly BaseDbContext _dbContext;

    public InboxStore(BaseDbContext dbContext)
    {
        _dbContext = dbContext;
    }
}
