using OrangeHRM.Tests.Fixtures;
using OrangeHRM.Tests.Pages;
using OrangeHRM.Tests.TestData;
using static Microsoft.Playwright.Assertions;

namespace OrangeHRM.Tests.Tests.Pim;

public class EmployeeDeletionTests : PimTestBase
{
    [Test]
    [Description("Create an employee via API, then test cancel and confirm deletion in the UI.")]
    public async Task EmployeeIsDeletedOnlyAfterConfirmation()
    {
        var employeeList = new EmployeeListPage(Page);
        var addEmployee = new AddEmployeePage(Page);
        var employee = EmployeeData.CreateUnique();
        EmployeesToCleanup.Add(employee);

        var employeeData = new
        {
            firstName = employee.FirstName,
            middleName = employee.MiddleName,
            lastName = employee.LastName,
            employeeId = employee.EmployeeId,
            empPicture = (string?)null
        };

        var response = await Context.APIRequest.PostAsync(
            "https://opensource-demo.orangehrmlive.com/web/index.php/api/v2/pim/employees",
            new() { DataObject = employeeData });
        await Expect(response).ToBeOKAsync();

        await Dashboard.OpenPimModuleAsync();
        await employeeList.OpenEmployeeListAsync();
        await employeeList.SearchByEmployeeIdAsync(employee.EmployeeId);
        await employeeList.VerifyEmployeeIsListedAsync(employee);
        await employeeList.OpenDeleteConfirmationAsync(employee);
        await employeeList.VerifyDeleteConfirmationIsDisplayedAsync();
        await employeeList.CancelEmployeeDeletionAsync();

        // Cancel should keep the employee.
        await employeeList.SearchByEmployeeIdAsync(employee.EmployeeId);
        await employeeList.VerifyEmployeeIsListedAsync(employee);
        await employeeList.OpenDeleteConfirmationAsync(employee);
        await employeeList.VerifyDeleteConfirmationIsDisplayedAsync();
        await employeeList.ConfirmEmployeeDeletionAsync();

        // Reopen the list and check the employee was deleted.
        await employeeList.OpenAddEmployeeFormAsync();
        await addEmployee.VerifyPageIsDisplayedAsync();
        await employeeList.OpenEmployeeListAsync();
        await employeeList.SearchByEmployeeIdAsync(employee.EmployeeId);
        await employeeList.VerifyEmployeeIdIsNotListedAsync(employee.EmployeeId);
    }
}
