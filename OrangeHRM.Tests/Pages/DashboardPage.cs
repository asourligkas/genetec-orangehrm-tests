using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace OrangeHRM.Tests.Pages;

public class DashboardPage(IPage page) : BasePage(page)
{
    private ILocator Heading => Page.GetByRole(AriaRole.Heading, new() { Name = "Dashboard", Exact = true });
    private ILocator PimModuleLink => Page.GetByRole(AriaRole.Link, new() { Name = "PIM", Exact = true });

    public async Task VerifyPageIsDisplayedAsync()
    {
        await Expect(Heading).ToBeVisibleAsync();
    }

    public async Task OpenPimModuleAsync()
    {
        await PimModuleLink.ClickAsync();
    }
}
