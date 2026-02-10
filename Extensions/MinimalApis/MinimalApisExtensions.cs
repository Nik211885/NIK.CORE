using Hangfire;
using Microsoft.AspNetCore.Routing;

namespace NIK.CORE.DOMAIN.Extensions.MinimalApis;

public static class MinimalApisExtensions
{
    extension(IEndpointRouteBuilder endpoints)
    {
        public IEndpointRouteBuilder UseCoreApis()
        {
            endpoints.MapHangfireJobDashboard();
            return endpoints;
        }
    }
}
