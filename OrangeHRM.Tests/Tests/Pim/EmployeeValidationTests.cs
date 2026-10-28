using OrangeHRM.Tests.Fixtures;
using OrangeHRM.Tests.Pages;
using OrangeHRM.Tests.TestData;

namespace OrangeHRM.Tests.Tests.Pim;

public class EmployeeValidationTests : PimTestBase
{
    [Test]
    public async Task RequiredNamesMustBeProvidedBeforeEmployeeCanBeCreated()
    {
        var employeeList = new EmployeeListPage(Page);
        var addEmployee = new AddEmployeePage(Page);
        var employeeDetails = new EmployeeDetailsPage(Page);

        await Dashboard.OpenPimModuleAsync();
        await employeeList.OpenAddEmployeeFormAsync();
        await addEmployee.VerifyPageIsDisplayedAsync();

        // Track the intended ID before any Save, including invalid submissions.
        _employee = EmployeeData.CreateUnique();
        TestContext.Progress.WriteLine($"Validation test employee: {_employee.EmployeeId}.");
        await addEmployee.SetEmployeeIdAsync(_employee.EmployeeId);
        await addEmployee.VerifyNamesAreEmptyAsync();

        await addEmployee.SubmitAsync();
        await addEmployee.VerifyRequiredNameErrorsAsync();

        await addEmployee.FillFirstNameAsync(_employee.FirstName);
        await addEmployee.VerifyFirstNameIsValidAndLastNameIsRequiredAsync(_employee.FirstName);

        await addEmployee.FillLastNameAsync(_employee.LastName);
        await addEmployee.SubmitAsync();
        await employeeDetails.VerifyPageIsDisplayedAsync();
        await employeeDetails.VerifyEmployeeIdAsync(_employee.EmployeeId);
        await employeeDetails.VerifyEmployeeNameAsync(_employee.FirstName, _employee.LastName);
    }
}
