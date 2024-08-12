using Microsoft.Extensions.Configuration;

namespace CodingTracker.Utilities
{
    public static class Configuration
    {
        public static string GetConnectionString()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            
            return config.GetSection("Database").GetSection("ConnectionString").Value
                   ?? "DefaultConnectionString";
        }
    }
}
