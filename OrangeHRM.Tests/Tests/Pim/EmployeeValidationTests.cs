using OrangeHRM.Tests.Fixtures;
using OrangeHRM.Tests.Pages;
using OrangeHRM.Tests.TestData;

namespace OrangeHRM.Tests.Tests.Pim;

public class EmployeeValidationTests : PimTestBase
{
    [Test]
    [Description("Verify required errors for both names, correct each in turn, and confirm employee creation.")]
    public async Task RequiredNamesMustBeProvidedBeforeEmployeeCanBeCreated()
    {
        var employeeList = new EmployeeListPage(Page);
        var addEmployee = new AddEmployeePage(Page);
        var employeeDetails = new EmployeeDetailsPage(Page);

        await Dashboard.OpenPimModuleAsync();
        await employeeList.OpenAddEmployeeFormAsync();
        await addEmployee.VerifyPageIsDisplayedAsync();

        // Track the intended ID before any Save, including invalid submissions.
        var employee = EmployeeData.CreateUnique();
        EmployeesToCleanup.Add(employee);
        await addEmployee.SetEmployeeIdAsync(employee.EmployeeId);
        await addEmployee.VerifyNamesAreEmptyAsync();

        await addEmployee.SubmitAsync();
        await addEmployee.VerifyRequiredNameErrorsAsync();

        await addEmployee.FillFirstNameAsync(employee.FirstName);
        await addEmployee.VerifyFirstNameIsValidAndLastNameIsRequiredAsync(employee.FirstName);

        await addEmployee.FillLastNameAsync(employee.LastName);
        await addEmployee.SubmitAsync();
        await employeeDetails.VerifyPageIsDisplayedAsync();
        await employeeDetails.VerifyEmployeeIdAsync(employee.EmployeeId);
        await employeeDetails.VerifyEmployeeNameAsync(employee.FirstName, employee.LastName);
    }

    [Test]
    [Description("Verify that a duplicate employee ID prevents creation, then use a unique ID to create the new employee without changing the original record.")]
    public async Task CannotCreateEmployeeWithDuplicateId()
    {
        var employeeList = new EmployeeListPage(Page);
        var addEmployee = new AddEmployeePage(Page);
        var employeeDetails = new EmployeeDetailsPage(Page);

        var firstEmployee = EmployeeData.CreateUnique();
        var secondEmployee = EmployeeData.CreateUnique();
        // Track both intended IDs before Save so partial creation failures are cleaned up.
        EmployeesToCleanup.Add(firstEmployee);
        EmployeesToCleanup.Add(secondEmployee);

        await Dashboard.OpenPimModuleAsync();
        await employeeList.OpenAddEmployeeFormAsync();
        await addEmployee.CreateEmployeeAsync(firstEmployee);
        await employeeDetails.VerifyPageIsDisplayedAsync();
        await employeeDetails.VerifyEmployeeIdAsync(firstEmployee.EmployeeId);
        await employeeDetails.VerifyEmployeeNameAsync(firstEmployee.FirstName, firstEmployee.LastName);

        await employeeList.OpenAddEmployeeFormAsync();
        var duplicateEmployee = secondEmployee with { EmployeeId = firstEmployee.EmployeeId };
        // Also clean up B under A's ID if duplicate creation unexpectedly succeeds.
        EmployeesToCleanup.Add(duplicateEmployee);
        await addEmployee.FillEmployeeDetailsAsync(duplicateEmployee);
        await addEmployee.SubmitAsync();
        await addEmployee.VerifyDuplicateEmployeeIdErrorAsync(firstEmployee.EmployeeId);

        // Recover on the same form, retaining the second employee's names.
        await addEmployee.SetEmployeeIdAsync(secondEmployee.EmployeeId);
        await addEmployee.VerifyEmployeeIdErrorIsClearedAsync(secondEmployee.EmployeeId);
        await addEmployee.SubmitAsync();
        await employeeDetails.VerifyPageIsDisplayedAsync();
        await employeeDetails.VerifyEmployeeIdAsync(secondEmployee.EmployeeId);
        await employeeDetails.VerifyEmployeeNameAsync(secondEmployee.FirstName, secondEmployee.LastName);

        await employeeList.OpenEmployeeListAsync();
        await employeeList.SearchByEmployeeIdAsync(firstEmployee.EmployeeId);
        // Exactly one original row must remain, with its original name.
        await employeeList.VerifyEmployeeIsListedAsync(firstEmployee);
        await employeeList.SearchByEmployeeIdAsync(secondEmployee.EmployeeId);
        await employeeList.VerifyEmployeeIsListedAsync(secondEmployee);
    }
}
