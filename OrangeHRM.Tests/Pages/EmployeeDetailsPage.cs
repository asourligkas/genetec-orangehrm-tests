using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace OrangeHRM.Tests.Pages;

public class EmployeeDetailsPage(IPage page) : BasePage(page)
{
    private ILocator Heading => Page.GetByRole(AriaRole.Heading, new() { Name = "Personal Details", Exact = true });
    private ILocator FirstNameInput => Page.GetByPlaceholder("First Name", new() { Exact = true });
    private ILocator MiddleNameInput => Page.GetByPlaceholder("Middle Name", new() { Exact = true });
    private ILocator LastNameInput => Page.GetByPlaceholder("Last Name", new() { Exact = true });
    private ILocator EmployeeIdInput => Page.Locator(".oxd-input-group")
        .Filter(new() { Has = Page.GetByText("Employee Id", new() { Exact = true }) })
        .GetByRole(AriaRole.Textbox);
    private ILocator OtherIdInput => Page.Locator(".oxd-input-group")
        .Filter(new() { Has = Page.GetByText("Other Id", new() { Exact = true }) })
        .GetByRole(AriaRole.Textbox);
    private ILocator PersonalDetailsForm => Page.Locator("form").Filter(new() { Has = MiddleNameInput });
    private ILocator SaveButton => PersonalDetailsForm.GetByRole(AriaRole.Button, new() { Name = "Save", Exact = true });
    private ILocator SaveConfirmation => Page.GetByText("Successfully Updated", new() { Exact = true });

    public async Task VerifyPageIsDisplayedAsync()
    {
        await Expect(Heading).ToBeVisibleAsync();
        await Expect(FirstNameInput).ToBeEditableAsync();
        await Expect(MiddleNameInput).ToBeEditableAsync();
        await Expect(LastNameInput).ToBeEditableAsync();
        await Expect(OtherIdInput).ToBeEditableAsync();
        await Expect(SaveButton).ToBeEnabledAsync();
    }

    public async Task VerifyEmployeeIdAsync(string employeeId)
    {
        await Expect(EmployeeIdInput).ToHaveValueAsync(employeeId);
    }

    public async Task UpdateLastNameAndOtherIdAsync(string lastName, string otherId)
    {
        await LastNameInput.FillAsync(lastName);
        await OtherIdInput.FillAsync(otherId);
        await SaveButton.ClickAsync();
    }

    public async Task VerifySaveConfirmationAsync()
    {
        await Expect(SaveConfirmation).ToBeVisibleAsync();
    }
}
