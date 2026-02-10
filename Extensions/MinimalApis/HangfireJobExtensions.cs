using Hangfire;
using Hangfire.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NIK.CORE.DOMAIN.Extensions.ApplicationBuilder;

namespace NIK.CORE.DOMAIN.Extensions.MinimalApis;

public static class HangfireJobExtensions
{
    extension(IEndpointRouteBuilder endpoints)
    {
        public IEndpointRouteBuilder MapHangfireJobDashboard()
        {
            var group = endpoints.MapGroup("/api/admin/jobs")
                .WithTags("Hangfire Management");
            
            group.MapGet("/", () =>
            {
                using var connection = JobStorage.Current.GetConnection();
                var recurringJobs = connection.GetRecurringJobs();
            
                var result = recurringJobs.Select(j => new
                {
                    j.Id,
                    j.Cron,
                    j.Queue,
                    j.LastExecution,
                    j.NextExecution,
                    j.LastJobState,
                    CreatedAt = j.CreatedAt?.ToString("yyyy-MM-dd HH:mm:ss")
                });

                return Results.Ok(result);
            }).WithName("GetAllJobs");
            
            group.MapPost("/{id}/trigger", (string id, IRecurringJobManager recurringJob) =>
            {
                recurringJob.Trigger(id);
                return Results.Ok(new { Message = $"Job '{id}' triggered successfully." });
            }).WithName("TriggerJob");
            
            group.MapDelete("/{id}", (string id, IRecurringJobManager recurringJob) =>
            {
                recurringJob.RemoveIfExists(id);
                return Results.Ok(new { Message = $"Job '{id}' removed." });
            }).WithName("DeleteJob");
            
            group.MapPost("/reset", (IRecurringJobManager recurringJob) =>
                {
                    JobScheduler.ScheduleJobs(recurringJob);
                    return Results.Ok(new { Message = "All jobs default re-registered." });
                })
                .WithName("ResetHangfireJobs"); 
            
            group.MapPost("reset-in-outbox", (IRecurringJobManager recurringJob) =>
            {
                JobScheduler.ScheduleJobsForInOutBox(recurringJob);      
                return Results.Ok(new { Message = "All In-Outbox jobs re-registered." });
            }).WithName("ResetInOutboxJobs");
            return endpoints;
        }
    }
}
