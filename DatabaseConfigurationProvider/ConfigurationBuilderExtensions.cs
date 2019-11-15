using Microsoft.Extensions.Configuration;

namespace DatabaseConfigurationProvider
{
    internal static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddDatabaseProvider(this IConfigurationBuilder configuration, string connectionString)
        {
            configuration.Add(new DatabaseConfigurationSource(connectionString));
            return configuration;
        }
    }
}
