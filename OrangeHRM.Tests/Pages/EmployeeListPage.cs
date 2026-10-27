using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace OrangeHRM.Tests.Pages;

public class EmployeeListPage(IPage page) : BasePage(page)
{
    private ILocator Heading => Page.GetByRole(AriaRole.Heading, new() { Name = "Employee Information", Exact = true });
    private ILocator AddEmployeeLink => Page.GetByRole(AriaRole.Link, new() { Name = "Add Employee", Exact = true });

    public async Task VerifyPageIsDisplayedAsync()
    {
        await Expect(Heading).ToBeVisibleAsync();
    }

    public async Task OpenAddEmployeeFormAsync()
    {
        await AddEmployeeLink.ClickAsync();
    }
}
