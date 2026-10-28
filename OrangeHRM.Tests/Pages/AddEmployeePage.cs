using Microsoft.Playwright;
using OrangeHRM.Tests.TestData;
using static Microsoft.Playwright.Assertions;

namespace OrangeHRM.Tests.Pages;

public class AddEmployeePage(IPage page) : BasePage(page)
{
    private ILocator Heading => Page.GetByRole(AriaRole.Heading, new() { Name = "Add Employee", Exact = true });
    private ILocator FirstNameInput => Page.GetByPlaceholder("First Name", new() { Exact = true });
    private ILocator MiddleNameInput => Page.GetByPlaceholder("Middle Name", new() { Exact = true });
    private ILocator LastNameInput => Page.GetByPlaceholder("Last Name", new() { Exact = true });
    // The label has no 'for' attribute, so GetByLabel cannot identify this input.
    private ILocator EmployeeIdInput => Page.Locator(".oxd-input-group")
        .Filter(new() { Has = Page.GetByText("Employee Id", new() { Exact = true }) })
        .GetByRole(AriaRole.Textbox);
    private ILocator CreateLoginDetailsCheckbox => Page.GetByRole(AriaRole.Checkbox);
    private ILocator SaveButton => Page.GetByRole(AriaRole.Button, new() { Name = "Save", Exact = true });

    public async Task VerifyPageIsDisplayedAsync()
    {
        await Expect(Heading).ToBeVisibleAsync();
        await Expect(FirstNameInput).ToBeEditableAsync();
        await Expect(MiddleNameInput).ToBeEditableAsync();
        await Expect(LastNameInput).ToBeEditableAsync();
    }

    public async Task CreateEmployeeAsync(EmployeeData employee)
    {
        await FirstNameInput.FillAsync(employee.FirstName);
        await MiddleNameInput.FillAsync(employee.MiddleName);
        await LastNameInput.FillAsync(employee.LastName);
        await EmployeeIdInput.FillAsync(employee.EmployeeId);
        await CreateLoginDetailsCheckbox.UncheckAsync();
        await SaveButton.ClickAsync();
        await Heading.WaitForAsync(new() { State = WaitForSelectorState.Hidden });
    }
}
