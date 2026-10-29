using NUnit.Framework.Interfaces;
using OrangeHRM.Tests.Configuration;
using OrangeHRM.Tests.Pages;
using OrangeHRM.Tests.TestData;

namespace OrangeHRM.Tests.Fixtures;

public abstract class PimTestBase : BaseTest
{
    protected List<EmployeeData> EmployeesToCleanup { get; } = [];

    [SetUp]
    public async Task LoginBeforeTest()
    {
        await Login.OpenAsync();
        await Login.LoginAsync(TestSettings.Username, TestSettings.Password);
        await Dashboard.VerifyPageIsDisplayedAsync();
    }

    [TearDown]
    public async Task DeleteCreatedEmployees()
    {
        // Preserve the failed page before cleanup navigates to the employee list.
        await CaptureFailureScreenshotAsync();

        var cleanupFailures = new List<Exception>();
        foreach (var employee in EmployeesToCleanup)
        {
            try
            {
                var employeeList = new EmployeeListPage(Page);
                await Dashboard.OpenPimModuleAsync();
                await employeeList.OpenEmployeeListAsync();
                await employeeList.DeleteEmployeeIfPresentAsync(employee);
                await employeeList.SearchByEmployeeIdAsync(employee.EmployeeId);
                await employeeList.VerifyEmployeeIsNotListedAsync(employee);
                TestContext.Progress.WriteLine($"Cleanup verified for employee {employee.EmployeeId} / {employee.FirstName}.");
            }
            catch (Exception exception)
            {
                var message = $"Cleanup failed for employee {employee.EmployeeId} / {employee.FirstName}: {exception.Message}";
                TestContext.Error.WriteLine(message);
                cleanupFailures.Add(new InvalidOperationException(message, exception));
            }
        }

        EmployeesToCleanup.Clear();
        if (cleanupFailures.Count > 0 && TestContext.CurrentContext.Result.Outcome.Status != TestStatus.Failed)
        {
            throw new AggregateException("Employee cleanup failed.", cleanupFailures);
        }
    }
}
