namespace OrangeHRM.Tests.Configuration;

public static class TestSettings
{
    // Defaults are the public OrangeHRM demo settings. Environment variables override them.
    public static string BaseUrl =>
        Environment.GetEnvironmentVariable("ORANGEHRM_BASE_URL")
        ?? "https://opensource-demo.orangehrmlive.com/";

    public static string Username =>
        Environment.GetEnvironmentVariable("ORANGEHRM_USERNAME") ?? "Admin";

    public static string Password =>
        Environment.GetEnvironmentVariable("ORANGEHRM_PASSWORD") ?? "admin123";
}
