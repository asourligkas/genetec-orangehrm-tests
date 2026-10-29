using OrangeHRM.Tests.Fixtures;
using OrangeHRM.Tests.Pages;
using OrangeHRM.Tests.TestData;

namespace OrangeHRM.Tests.Tests.Pim;

public class EmployeeWorkflowTests : PimTestBase
{
    [Test]
    public async Task AdminCanCreateFindEditAndVerifyEmployeeDetails()
    {
        var employeeList = new EmployeeListPage(Page);
        var addEmployee = new AddEmployeePage(Page);
        var employeeDetails = new EmployeeDetailsPage(Page);

        await Dashboard.OpenPimModuleAsync();
        await employeeList.VerifyPageIsDisplayedAsync();

        await employeeList.OpenAddEmployeeFormAsync();
        await addEmployee.VerifyPageIsDisplayedAsync();

        // Register before Save so teardown can find a record even if the subsequent wait fails.
        var employee = EmployeeData.CreateUnique();
        EmployeesToCleanup.Add(employee);
        TestContext.Progress.WriteLine($"Test employee: {employee.EmployeeId} / {employee.FirstName} {employee.LastName}");
        await addEmployee.CreateEmployeeAsync(employee);

        await employeeList.OpenEmployeeListAsync();
        await employeeList.SearchByEmployeeIdAsync(employee.EmployeeId);
        await employeeList.VerifyEmployeeIsListedAsync(employee);
        await employeeList.OpenEmployeeDetailsAsync(employee.EmployeeId);
        await employeeDetails.VerifyPageIsDisplayedAsync();
        await employeeDetails.VerifyEmployeeIdAsync(employee.EmployeeId);
        var dateOfBirth = new DateOnly(1990, 5, 17);
        var updatedEmployee = employee with
        {
            LastName = "DemoUpdated",
            OtherId = $"ALT{employee.EmployeeId}",
            Nationality = "Canadian",
            DateOfBirth = dateOfBirth
        };
        await employeeDetails.UpdateLastNameAndOtherIdAsync(updatedEmployee.LastName, updatedEmployee.OtherId);
        await employeeDetails.SelectNationalityAsync(updatedEmployee.Nationality);
        await employeeDetails.SelectDateOfBirthAsync(dateOfBirth);
        await employeeDetails.SavePersonalDetailsAsync();
        await employeeDetails.VerifySaveConfirmationAsync();
        employee = updatedEmployee;

        await employeeList.OpenEmployeeListAsync();
        await employeeList.SearchByEmployeeIdAsync(employee.EmployeeId);
        await employeeList.VerifyEmployeeIsListedAsync(employee);
        await employeeList.OpenEmployeeDetailsAsync(employee.EmployeeId);
        await employeeDetails.VerifyPageIsDisplayedAsync();
        await employeeDetails.VerifyEmployeeIdAsync(employee.EmployeeId);
        await employeeDetails.VerifyLastNameAndOtherIdAsync(employee.LastName, employee.OtherId);
        await employeeDetails.VerifyNationalityAsync(employee.Nationality);
        await employeeDetails.VerifyDateOfBirthAsync(dateOfBirth);
    }
}
