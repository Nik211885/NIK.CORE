using Microsoft.Extensions.DependencyInjection;
using NIK.CORE.DOMAIN.Configs;

namespace NIK.CORE.DOMAIN.Extensions.Dependency;

public static class DependencyServicesDefaultExtensions
{
    extension(IServiceCollection serviceCollection)
    {
        IServiceCollection AddServicesDefault()
        {
            serviceCollection.AddScoped<IConfigManager, ConfigManager>();
            return serviceCollection;
        }
    }
}
