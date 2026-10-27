namespace OrangeHRM.Tests.Configuration;

public static class TestSettings
{
    // Defaults are the public OrangeHRM demo settings. Environment variables override them.
    public static string BaseUrl
    {
        get
        {
            var value = ReadSetting("ORANGEHRM_BASE_URL", "https://opensource-demo.orangehrmlive.com/");
            if (!Uri.TryCreate(value, UriKind.Absolute, out var uri)
                || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                throw new InvalidOperationException(
                    "ORANGEHRM_BASE_URL must be an absolute HTTP or HTTPS URL.");
            }

            return value;
        }
    }

    public static string Username =>
        ReadSetting("ORANGEHRM_USERNAME", "Admin");

    public static string Password =>
        ReadSetting("ORANGEHRM_PASSWORD", "admin123");

    private static string ReadSetting(string name, string defaultValue)
    {
        var value = Environment.GetEnvironmentVariable(name) ?? defaultValue;
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"{name} must not be empty or whitespace.");
        }

        return value;
    }
}
