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
    // Each message is a sibling of its input wrapper, with no aria-describedby association.
    private ILocator FirstNameValidationError => FirstNameInput.Locator("xpath=../..")
        .Locator(".oxd-input-field-error-message");
    private ILocator LastNameValidationError => LastNameInput.Locator("xpath=../..")
        .Locator(".oxd-input-field-error-message");

    public async Task VerifyPageIsDisplayedAsync()
    {
        await Expect(Heading).ToBeVisibleAsync();
        await Expect(FirstNameInput).ToBeEditableAsync();
        await Expect(MiddleNameInput).ToBeEditableAsync();
        await Expect(LastNameInput).ToBeEditableAsync();
        await Expect(EmployeeIdInput).Not.ToHaveValueAsync("");
    }

    public async Task VerifyNamesAreEmptyAsync()
    {
        await Expect(FirstNameInput).ToHaveValueAsync("");
        await Expect(LastNameInput).ToHaveValueAsync("");
    }

    public async Task VerifyRequiredNameErrorsAsync()
    {
        await Expect(Heading).ToBeVisibleAsync();
        await Expect(FirstNameValidationError).ToBeVisibleAsync();
        await Expect(FirstNameValidationError).ToHaveTextAsync("Required");
        await Expect(LastNameValidationError).ToBeVisibleAsync();
        await Expect(LastNameValidationError).ToHaveTextAsync("Required");
    }

    public async Task VerifyFirstNameIsValidAndLastNameIsRequiredAsync(string firstName)
    {
        await Expect(Heading).ToBeVisibleAsync();
        await Expect(FirstNameInput).ToHaveValueAsync(firstName);
        await Expect(FirstNameValidationError).ToBeHiddenAsync();
        await Expect(LastNameInput).ToHaveValueAsync("");
        await Expect(LastNameValidationError).ToBeVisibleAsync();
        await Expect(LastNameValidationError).ToHaveTextAsync("Required");
    }

    public async Task CreateEmployeeAsync(EmployeeData employee)
    {
        await FillFirstNameAsync(employee.FirstName);
        await MiddleNameInput.FillAsync(employee.MiddleName);
        await FillLastNameAsync(employee.LastName);
        await SetEmployeeIdAsync(employee.EmployeeId);
        await CreateLoginDetailsCheckbox.UncheckAsync();
        await SubmitAsync();
        await Heading.WaitForAsync(new() { State = WaitForSelectorState.Hidden });
    }

    public async Task FillFirstNameAsync(string firstName)
    {
        await FirstNameInput.FillAsync(firstName);
    }

    public async Task FillLastNameAsync(string lastName)
    {
        await LastNameInput.FillAsync(lastName);
    }

    public async Task SetEmployeeIdAsync(string employeeId)
    {
        await EmployeeIdInput.FillAsync(employeeId);
    }

    public async Task SubmitAsync()
    {
        await SaveButton.ClickAsync();
    }
}
