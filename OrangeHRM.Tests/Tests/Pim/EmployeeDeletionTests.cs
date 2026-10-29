using OrangeHRM.Tests.Fixtures;
using OrangeHRM.Tests.Pages;
using OrangeHRM.Tests.TestData;

namespace OrangeHRM.Tests.Tests.Pim;

public class EmployeeDeletionTests : PimTestBase
{
    [Test]
    [Description("Verify cancellation preserves the employee, then confirm deletion and check absence in a fresh search.")]
    public async Task EmployeeIsDeletedOnlyAfterConfirmation()
    {
        var employeeList = new EmployeeListPage(Page);
        var addEmployee = new AddEmployeePage(Page);
        var employeeDetails = new EmployeeDetailsPage(Page);
        var employee = EmployeeData.CreateUnique();
        EmployeesToCleanup.Add(employee);

        await Dashboard.OpenPimModuleAsync();
        await employeeList.OpenAddEmployeeFormAsync();
        await addEmployee.CreateEmployeeAsync(employee);
        await employeeDetails.VerifyPageIsDisplayedAsync();
        await employeeDetails.VerifyEmployeeIdAsync(employee.EmployeeId);

        await employeeList.OpenEmployeeListAsync();
        await employeeList.SearchByEmployeeIdAsync(employee.EmployeeId);
        await employeeList.VerifyEmployeeIsListedAsync(employee);
        await employeeList.OpenDeleteConfirmationAsync(employee);
        await employeeList.VerifyDeleteConfirmationIsDisplayedAsync();
        await employeeList.CancelEmployeeDeletionAsync();

        // Fetch the employee again to verify cancellation preserved the record and its name.
        await employeeList.SearchByEmployeeIdAsync(employee.EmployeeId);
        await employeeList.VerifyEmployeeIsListedAsync(employee);
        await employeeList.OpenDeleteConfirmationAsync(employee);
        await employeeList.VerifyDeleteConfirmationIsDisplayedAsync();
        await employeeList.ConfirmEmployeeDeletionAsync();

        // Leave and reopen the list to verify absence in a fresh view.
        await employeeList.OpenAddEmployeeFormAsync();
        await addEmployee.VerifyPageIsDisplayedAsync();
        await employeeList.OpenEmployeeListAsync();
        await employeeList.SearchByEmployeeIdAsync(employee.EmployeeId);
        await employeeList.VerifyEmployeeIdIsNotListedAsync(employee.EmployeeId);
    }
}
