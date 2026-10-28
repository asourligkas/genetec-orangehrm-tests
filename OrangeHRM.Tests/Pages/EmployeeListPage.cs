using Microsoft.Playwright;
using OrangeHRM.Tests.TestData;
using static Microsoft.Playwright.Assertions;

namespace OrangeHRM.Tests.Pages;

public class EmployeeListPage(IPage page) : BasePage(page)
{
    private ILocator Heading => Page.GetByRole(AriaRole.Heading, new() { Name = "Employee Information", Exact = true });
    private ILocator PimAddEmployeeMenuLink => Page.GetByRole(AriaRole.Link, new() { Name = "Add Employee", Exact = true });
    private ILocator PimEmployeeListMenuLink => Page.GetByRole(AriaRole.Link, new() { Name = "Employee List", Exact = true });
    private ILocator EmployeeIdInput => Page.Locator(".oxd-input-group")
        .Filter(new() { Has = Page.GetByText("Employee Id", new() { Exact = true }) })
        .GetByRole(AriaRole.Textbox);
    private ILocator SearchButton => Page.GetByRole(AriaRole.Button, new() { Name = "Search", Exact = true });
    // "No Records Found" appears in a results span and a toast paragraph; select only the results.
    private ILocator NoRecordsMessage => Page.Locator("span").Filter(new() { HasText = "No Records Found" });
    private ILocator ConfirmDeleteButton => Page.GetByRole(AriaRole.Dialog)
        .GetByRole(AriaRole.Button, new() { Name = "Yes, Delete" });

    private ILocator EmployeeRow(string employeeId) => Page.GetByRole(AriaRole.Row)
        .Filter(new() { Has = Page.GetByRole(AriaRole.Cell, new() { Name = employeeId, Exact = true }) });

    private ILocator DeleteButtonForEmployee(string employeeId) => EmployeeRow(employeeId)
        .GetByRole(AriaRole.Button).Filter(new() { Has = Page.Locator("i.bi-trash") });

    public async Task VerifyPageIsDisplayedAsync()
    {
        await Expect(Heading).ToBeVisibleAsync();
        await Expect(EmployeeIdInput).ToBeEditableAsync();
        await Expect(SearchButton).ToBeVisibleAsync();
        await Expect(SearchButton).ToBeEnabledAsync();
    }

    public async Task OpenAddEmployeeFormAsync()
    {
        await PimAddEmployeeMenuLink.ClickAsync();
    }

    public async Task OpenEmployeeListAsync()
    {
        if (!await Heading.IsVisibleAsync())
        {
            await PimEmployeeListMenuLink.ClickAsync();
        }

        await VerifyPageIsDisplayedAsync();
    }

    public async Task SearchByEmployeeIdAsync(string employeeId)
    {
        await EmployeeIdInput.FillAsync(employeeId);
        var response = await Page.RunAndWaitForResponseAsync(
            () => SearchButton.ClickAsync(), response => IsGetEmployeesResponse(response)
                && response.Url.Contains(Uri.EscapeDataString(employeeId), StringComparison.Ordinal));
        await response.FinishedAsync();
        await Expect(EmployeeRow(employeeId).Or(NoRecordsMessage)).ToBeVisibleAsync();
    }

    public async Task VerifyEmployeeIsListedAsync(EmployeeData employee)
    {
        var row = EmployeeRow(employee.EmployeeId);
        var firstAndMiddleName = $"{employee.FirstName} {employee.MiddleName}".Trim();
        await Expect(row).ToHaveCountAsync(1);
        await Expect(row.GetByRole(AriaRole.Cell, new() { Name = firstAndMiddleName, Exact = true })).ToBeVisibleAsync();
        await Expect(row.GetByRole(AriaRole.Cell, new() { Name = employee.LastName, Exact = true })).ToBeVisibleAsync();
    }

    public async Task OpenEmployeeDetailsAsync(string employeeId)
    {
        await EmployeeRow(employeeId).GetByRole(AriaRole.Cell, new() { Name = employeeId, Exact = true }).ClickAsync();
    }

    public async Task VerifyEmployeeIsNotListedAsync(string employeeId)
    {
        await Expect(NoRecordsMessage).ToBeVisibleAsync();
        await Expect(EmployeeRow(employeeId)).ToHaveCountAsync(0);
    }

    public async Task DeleteEmployeeIfPresentAsync(EmployeeData employee)
    {
        await OpenEmployeeListAsync();
        await SearchByEmployeeIdAsync(employee.EmployeeId);
        if (await NoRecordsMessage.IsVisibleAsync())
            return;

        var row = EmployeeRow(employee.EmployeeId);
        await Expect(row).ToHaveCountAsync(1);
        await DeleteButtonForEmployee(employee.EmployeeId).ClickAsync();
        await ConfirmDeleteButton.ClickAsync();
        await Expect(ConfirmDeleteButton).ToBeHiddenAsync();
    }

    private static bool IsGetEmployeesResponse(IResponse response) =>
        response.Request.Method == "GET"
        && new Uri(response.Url).AbsolutePath.EndsWith("/api/v2/pim/employees", StringComparison.Ordinal);
}
