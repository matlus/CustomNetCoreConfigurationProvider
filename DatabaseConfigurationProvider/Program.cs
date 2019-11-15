using Microsoft.Extensions.Configuration;
using System;

namespace DatabaseConfigurationProvider
{
    internal static class Program
    {
        private const string appSettingsSection = "AppSettings";
        static void Main(string[] args)
        {
            ////var databaseConfigurationSource = new DatabaseConfigurationSource(@"Data Source=(localdb)\ProjectsV13;Initial Catalog=ConfigurationStore;Integrated Security=True;TrustServerCertificate=True;");
            ////var databaseConfigurationProvider = databaseConfigurationSource.Build(null);
            ////databaseConfigurationProvider.Set($"{appSettingsSection}:A", "1");
            ////databaseConfigurationProvider.Set($"{appSettingsSection}:B", "2");
            ////databaseConfigurationProvider.Set($"{appSettingsSection}:C", "3");


            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddDatabaseProvider(@"Data Source=(localdb)\ProjectsV13;Initial Catalog=ConfigurationStore;Integrated Security=True;TrustServerCertificate=True;");
            var configurationRoot = configurationBuilder.Build();
            Console.WriteLine("The appSettings value for A is:" + configurationRoot.GetSection($"{appSettingsSection}:A").Value);
        }
    }
}
