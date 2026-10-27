using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace OrangeHRM.Tests.Pages;

public class AddEmployeePage(IPage page) : BasePage(page)
{
    private ILocator Heading => Page.GetByRole(AriaRole.Heading, new() { Name = "Add Employee", Exact = true });
    private ILocator FirstNameInput => Page.GetByPlaceholder("First Name", new() { Exact = true });
    private ILocator LastNameInput => Page.GetByPlaceholder("Last Name", new() { Exact = true });

    public async Task VerifyFormIsReadyAsync()
    {
        await Expect(Heading).ToBeVisibleAsync();
        await Expect(FirstNameInput).ToBeEditableAsync();
        await Expect(LastNameInput).ToBeEditableAsync();
    }
}
