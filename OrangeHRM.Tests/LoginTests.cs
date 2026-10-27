
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using OrangeHRM.Tests.Configuration;
using OrangeHRM.Tests.Pages;

namespace OrangeHRM.Tests;

public class LoginTests : PageTest
{
    [Test]
    public async Task AdminCanLoginSuccessfully()
    {
        var loginPage = new LoginPage(Page);
        await loginPage.NavigateAsync(TestSettings.BaseUrl);
        await loginPage.LoginAsync(TestSettings.Username, TestSettings.Password);

        // Verify successful login
        await Expect(
            Page.GetByRole(AriaRole.Heading, new() { Name = "Dashboard" })
        ).ToBeVisibleAsync(new() { Timeout = 15_000 });
    }
}
