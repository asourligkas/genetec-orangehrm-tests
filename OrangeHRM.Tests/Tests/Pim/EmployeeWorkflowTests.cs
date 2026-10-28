using OrangeHRM.Tests.Fixtures;
using OrangeHRM.Tests.Pages;
using OrangeHRM.Tests.TestData;
using NUnit.Framework.Interfaces;

namespace OrangeHRM.Tests.Tests.Pim;

public class EmployeeWorkflowTests : AuthenticatedTestBase
{
    private EmployeeData? _employee;

    [Test]
    public async Task AdminCanCreateFindAndEditEmployee()
    {
        var employeeList = new EmployeeListPage(Page);
        var addEmployee = new AddEmployeePage(Page);
        var employeeDetails = new EmployeeDetailsPage(Page);

        await Dashboard.OpenPimModuleAsync();
        await employeeList.VerifyPageIsDisplayedAsync();

        await employeeList.OpenAddEmployeeFormAsync();
        await addEmployee.VerifyPageIsDisplayedAsync();

        // Register before Save so teardown can find a record even if the subsequent wait fails.
        _employee = EmployeeData.CreateUnique();
        TestContext.Progress.WriteLine($"Test employee: {_employee.EmployeeId} / {_employee.FirstName} {_employee.LastName}");
        await addEmployee.CreateEmployeeAsync(_employee);

        await employeeList.OpenEmployeeListAsync();
        await employeeList.SearchByEmployeeIdAsync(_employee.EmployeeId);
        await employeeList.VerifyEmployeeIsListedAsync(_employee);
        await employeeList.OpenEmployeeDetailsAsync(_employee.EmployeeId);
        await employeeDetails.VerifyPageIsDisplayedAsync();
        await employeeDetails.VerifyEmployeeIdAsync(_employee.EmployeeId);
        var dateOfBirth = new DateOnly(1990, 5, 17);
        var updatedEmployee = _employee with
        {
            LastName = "DemoUpdated",
            OtherId = $"ALT{_employee.EmployeeId}",
            Nationality = "Canadian",
            DateOfBirth = dateOfBirth
        };
        await employeeDetails.UpdateLastNameAndOtherIdAsync(updatedEmployee.LastName, updatedEmployee.OtherId);
        await employeeDetails.SelectNationalityAsync(updatedEmployee.Nationality);
        await employeeDetails.SelectDateOfBirthAsync(dateOfBirth);
        await employeeDetails.SavePersonalDetailsAsync();
        await employeeDetails.VerifySaveConfirmationAsync();
        _employee = updatedEmployee;
    }

    [TearDown]
    public async Task DeleteCreatedEmployee()
    {
        if (_employee is null)
            return;

        try
        {
            var employeeList = new EmployeeListPage(Page);
            await Dashboard.OpenPimModuleAsync();
            await employeeList.OpenEmployeeListAsync();
            await employeeList.DeleteEmployeeIfPresentAsync(_employee);
            await employeeList.SearchByEmployeeIdAsync(_employee.EmployeeId);
            await employeeList.VerifyEmployeeIsNotListedAsync(_employee.EmployeeId);
            TestContext.Progress.WriteLine($"Cleanup verified for employee {_employee.EmployeeId}.");
        }
        catch (Exception exception)
        {
            var message = $"Cleanup failed for employee {_employee.EmployeeId}: {exception.Message}";
            if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
                TestContext.Error.WriteLine(message);
            else
                throw new InvalidOperationException(message, exception);
        }
        finally
        {
            _employee = null;
        }
    }
}
