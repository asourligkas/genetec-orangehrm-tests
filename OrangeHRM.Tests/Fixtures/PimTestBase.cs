using NUnit.Framework.Interfaces;
using OrangeHRM.Tests.Configuration;
using OrangeHRM.Tests.Pages;
using OrangeHRM.Tests.TestData;

namespace OrangeHRM.Tests.Fixtures;

public abstract class PimTestBase : BaseTest
{
    protected EmployeeData? _employee;

    [SetUp]
    public async Task LoginBeforeTest()
    {
        await Login.OpenAsync();
        await Login.LoginAsync(TestSettings.Username, TestSettings.Password);
        await Dashboard.VerifyPageIsDisplayedAsync();
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
