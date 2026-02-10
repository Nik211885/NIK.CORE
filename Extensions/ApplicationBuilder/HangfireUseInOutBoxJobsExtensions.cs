using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NIK.CORE.DOMAIN.Inbox;
using NIK.CORE.DOMAIN.Outbox;

namespace NIK.CORE.DOMAIN.Extensions.ApplicationBuilder;

public static class HangfireUseInOutBoxJobsExtensions
{
    extension(IApplicationBuilder app)
    {
        public IApplicationBuilder RegisterInOutBoxJobs()
        {
            var recursiveJob = app.ApplicationServices.GetRequiredService<IRecurringJobManager>();  
            JobScheduler.ScheduleJobs(recursiveJob);
            return app;
        }
    }
}


public static class JobScheduler
{
    public static void ScheduleJobs(IRecurringJobManager recurringJob)
    {
        JobScheduler.ScheduleJobsForInOutBox(recurringJob);
    }

    public static void ScheduleJobsForInOutBox(IRecurringJobManager recurringJobManager)
    {
        recurringJobManager.AddOrUpdate<OutboxProcessor>(
            "Messaging.Outbox.Processor", 
            job => job.ProcessAsync(CancellationToken.None, 100), 
            Cron.Minutely);
    
        recurringJobManager.AddOrUpdate<OutboxCleanupJob>(
            "Messaging.Outbox.Cleanup", 
            job => job.CleanJobAsync(CancellationToken.None, 7), 
            Cron.DayInterval(1));
    }
}
