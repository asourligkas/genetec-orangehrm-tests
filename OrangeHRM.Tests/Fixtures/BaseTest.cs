using Microsoft.Playwright.NUnit;
using NUnit.Framework.Interfaces;
using OrangeHRM.Tests.Pages;

namespace OrangeHRM.Tests.Fixtures;

[Parallelizable(ParallelScope.Self)]
public abstract class BaseTest : PageTest
{
    private static readonly string RunId = $"{DateTime.UtcNow:yyyy-MM-dd_HH-mm-ss-fff}Z-{Environment.ProcessId}";
    private string _artifactDirectory = null!;
    private string _testName = null!;
    private string _artifactFileName = null!;
    private string? _failureUrl;
    private bool _failureScreenshotCaptured;
    private bool _traceStarted;

    protected LoginPage Login { get; private set; } = null!;
    protected DashboardPage Dashboard { get; private set; } = null!;

    [SetUp]
    public async Task InitializePagesAndTracing()
    {
        _failureScreenshotCaptured = false;
        _traceStarted = false;
        _failureUrl = null;
        var test = TestContext.CurrentContext.Test;
        _testName = $"{GetType().Name}.{test.Name}";
        _artifactFileName = test.MethodName ?? "test";
        var safeTestName = string.Concat(_testName.Select(character =>
            Path.GetInvalidFileNameChars().Contains(character) ? '_' : character));
        _artifactDirectory = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.WorkDirectory, "artifacts", RunId,
            $"{safeTestName}-{Guid.NewGuid().ToString("N")[..8]}"));

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
            _failureUrl ??= Page.Url;
            Directory.CreateDirectory(_artifactDirectory);
            var path = Path.Combine(_artifactDirectory, $"{_artifactFileName}.png");
            await Page.ScreenshotAsync(new() { Path = path, FullPage = true, Timeout = 10000 });
            _failureScreenshotCaptured = true;
            TestContext.Out.WriteLine($"[{_testName}] Failure screenshot: {path}");
        }
        catch (Exception exception)
        {
            TestContext.Error.WriteLine($"[{_testName}] Screenshot capture failed: {exception.Message}");
        }
    }

    [TearDown]
    public async Task FinishReporting()
    {
        await CaptureFailureScreenshotAsync();

        if (_traceStarted)
        {
            try
            {
                if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
                {
                    Directory.CreateDirectory(_artifactDirectory);
                    var path = Path.Combine(_artifactDirectory, $"{_artifactFileName}.zip");
                    await Context.Tracing.StopAsync(new() { Path = path });
                    TestContext.Out.WriteLine($"[{_testName}] Failure trace (including cleanup): {path}");
                }
                else
                {
                    await Context.Tracing.StopAsync();
                }
            }
            catch (Exception exception)
            {
                TestContext.Error.WriteLine($"[{_testName}] Trace capture failed: {exception.Message}");
            }
        }

        if (TestContext.CurrentContext.Result.Outcome.Status != TestStatus.Failed)
            return;

        TestContext.Out.WriteLine($"[{_testName}] Page URL at failure: {_failureUrl}");
    }
}
