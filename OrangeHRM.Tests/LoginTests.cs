
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace OrangeHRM.Tests;

public class LoginTests : PageTest
{
    [Test]
    public async Task AdminCanLoginSuccessfully()
    {
        // Navigate to OrangeHRM
        await Page.GotoAsync("https://opensource-demo.orangehrmlive.com/");

        // Enter credentials
        await Page.GetByPlaceholder("Username").FillAsync("Admin");
        await Page.GetByPlaceholder("Password").FillAsync("admin123");

        // Login
        await Page.GetByRole(AriaRole.Button, new() { Name = "Login" })
            .ClickAsync();

        // Verify successful login
        await Expect(
            Page.GetByRole(AriaRole.Heading, new() { Name = "Dashboard" })
        ).ToBeVisibleAsync();
    }
}
