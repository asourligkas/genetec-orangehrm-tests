using OrangeHRM.Tests.Fixtures;
using OrangeHRM.Tests.Pages;

namespace OrangeHRM.Tests.Tests.Pim;

public class PimNavigationTests : AuthenticatedTestBase
{
    [Test]
    public async Task AdminCanOpenEmployeeCreationForm()
    {
        var employeeList = new EmployeeListPage(Page);
        var addEmployee = new AddEmployeePage(Page);

        await Dashboard.OpenPimModuleAsync();
        await employeeList.VerifyPageIsDisplayedAsync();

        await employeeList.OpenAddEmployeeFormAsync();
        await addEmployee.VerifyFormIsReadyAsync();
    }
}
