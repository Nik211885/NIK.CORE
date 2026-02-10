using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.DependencyInjection;
using NIK.CORE.DOMAIN.DataAccess;

namespace NIK.CORE.DOMAIN.Extensions.Dependency;

public static class DependencyHangfireExtension
{
    extension(IServiceCollection serviceCollection)
    {
        public IServiceCollection AddHangfire(Action<HangfireConfig> config)
        {
            var configObj = new HangfireConfig();
            config.Invoke(configObj);
            serviceCollection.AddHangfire(configuration =>
            {
                configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings();
                switch (configObj.DataBaseType)
                {
                    case DataBaseType.Postgres:
                        configuration.UsePostgreSqlStorage(options =>
                        {
                            options.UseNpgsqlConnection(configObj.ConnectionString);
                        });
                        break;
                    default:
                        throw new NotImplementedException($"Not implementation for database type {configObj.DataBaseType.ToString()}");
                }
            });
            serviceCollection.AddHangfireServer();
            return serviceCollection;
        }
    }
}


public sealed class HangfireConfig
{
    public DataBaseType DataBaseType { get; set; }
    public string ConnectionString { get; set; } = string.Empty;
}
