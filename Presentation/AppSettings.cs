using Microsoft.Extensions.Configuration;
using Presentation.Configuration;

namespace Presentation;

public static class AppSettings
{
    public static readonly AppSettingsRoot Root;

    static AppSettings()
    {
        Root = ConfigurationHelper.GetConfiguration().Get<AppSettingsRoot>()!;
    }

    #region setting classes

    public class AppSettingsRoot
    {
        public required Bot Bot { get; set; }
    }

    public class Bot
    {
        public required string Token { get; set; }
    }

    #endregion
}