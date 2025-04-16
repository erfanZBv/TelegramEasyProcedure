using Microsoft.Extensions.Configuration;

namespace Presentation.Configuration;

public static class ConfigurationHelper
{
    public static IConfigurationRoot GetConfiguration()
    {
        var directory = Directory.GetCurrentDirectory();
        var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(directory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);
        return configurationBuilder.Build();
    }
}