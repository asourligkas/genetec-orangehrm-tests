using Microsoft.Playwright.NUnit;
using OrangeHRM.Tests.Pages;

namespace OrangeHRM.Tests.Fixtures;

[Parallelizable(ParallelScope.Self)]
public abstract class BaseTest : PageTest
{
    protected LoginPage Login { get; private set; } = null!;
    protected DashboardPage Dashboard { get; private set; } = null!;

    [SetUp]
    public void InitializePages()
    {
        Login = new LoginPage(Page);
        Dashboard = new DashboardPage(Page);
    }
}
