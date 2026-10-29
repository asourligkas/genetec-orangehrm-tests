using NUnit.Framework;
using OrangeHRM.Tests.Configuration;
using OrangeHRM.Tests.Fixtures;

namespace OrangeHRM.Tests.Tests;

public class LoginTests : BaseTest
{
    [Test]
    [Description("Verify that valid administrator credentials open the dashboard.")]
    public async Task AdminCanLoginSuccessfully()
    {
        await Login.OpenAsync();
        await Login.LoginAsync(TestSettings.Username, TestSettings.Password);

        await Dashboard.VerifyPageIsDisplayedAsync();
    }
}
