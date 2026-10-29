using Microsoft.Playwright.NUnit;
using NUnit.Framework.Interfaces;
using OrangeHRM.Tests.Pages;

namespace OrangeHRM.Tests.Fixtures;

[Parallelizable(ParallelScope.Self)]
public abstract class BaseTest : PageTest
{
    private string _artifactDirectory = null!;
    private bool _failureScreenshotCaptured;
    private bool _traceStarted;

    protected LoginPage Login { get; private set; } = null!;
    protected DashboardPage Dashboard { get; private set; } = null!;

    [SetUp]
    public async Task InitializePagesAndTracing()
    {
        _failureScreenshotCaptured = false;
        _traceStarted = false;
        _artifactDirectory = Path.Combine(TestContext.CurrentContext.WorkDirectory,
            "TestResults", $"{GetType().Name}-{Guid.NewGuid():N}");

        Login = new LoginPage(Page);
        Dashboard = new DashboardPage(Page);

        await Context.Tracing.StartAsync(new()
        {
            Title = TestContext.CurrentContext.Test.FullName,
            Screenshots = true,
            Snapshots = true,
            Sources = true
        });
        _traceStarted = true;
    }

    protected async Task CaptureFailureScreenshotAsync()
    {
        if (_failureScreenshotCaptured || TestContext.CurrentContext.Result.Outcome.Status != TestStatus.Failed)
            return;

        try
        {
            Directory.CreateDirectory(_artifactDirectory);
            var path = Path.Combine(_artifactDirectory, "failure.png");
            await Page.ScreenshotAsync(new() { Path = path, FullPage = true, Timeout = 10000 });
            TestContext.AddTestAttachment(path, "Page at failure");
            _failureScreenshotCaptured = true;
        }
        catch (Exception exception)
        {
            TestContext.Error.WriteLine($"Could not capture failure screenshot: {exception.Message}");
        }
    }

    [TearDown]
    public async Task FinishReporting()
    {
        await CaptureFailureScreenshotAsync();

        if (!_traceStarted)
            return;

        try
        {
            if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
            {
                Directory.CreateDirectory(_artifactDirectory);
                var path = Path.Combine(_artifactDirectory, "trace.zip");
                await Context.Tracing.StopAsync(new() { Path = path });
                TestContext.AddTestAttachment(path, "Playwright trace including cleanup");
            }
            else
            {
                await Context.Tracing.StopAsync();
            }
        }
        catch (Exception exception)
        {
            TestContext.Error.WriteLine($"Could not finish Playwright trace: {exception.Message}");
        }
    }
}
