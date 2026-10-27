using OrangeHRM.Tests.Configuration;

namespace OrangeHRM.Tests.Fixtures;

public abstract class AuthenticatedTestBase : BaseTest
{
    [SetUp]
    public async Task LoginBeforeTest()
    {
        await Login.OpenAsync();
        await Login.LoginAsync(TestSettings.Username, TestSettings.Password);
        await Dashboard.VerifyPageIsDisplayedAsync();
    }
}
