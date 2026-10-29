# OrangeHRM PIM UI tests

QA assessment implemented with C#/.NET 10, NUnit, and Microsoft.Playwright.NUnit. C#/.NET was approved as an alternative to the originally requested Node.js framework.

Target: https://opensource-demo.orangehrmlive.com/

## Coverage

| Test | What it verifies |
| --- | --- |
| Administrator login | Valid credentials open the dashboard. |
| Employee workflow | Create an employee, search by ID, open personal details, edit last name, Other ID, nationality and date of birth, then reopen and verify persistence. |
| Required names | First and last name have separate required errors; correcting each field allows creation. |
| Duplicate employee ID | An existing ID prevents creation; correcting it allows a second employee to be created while preserving the original record. |
| Employee deletion | Cancel preserves the record; confirm removes it from a fresh search. |

Each PIM test logs in independently and creates its own test data. The three additional regression scenarios cover input validation, identifier uniqueness, and destructive-action confirmation.

## Setup and execution

Prerequisites: .NET 10 SDK and PowerShell 7 (`pwsh`). Run these commands from the repository root:

```powershell
dotnet restore OrangeHRM.Tests/OrangeHRM.Tests.csproj
dotnet build OrangeHRM.Tests/OrangeHRM.Tests.csproj --no-restore
pwsh OrangeHRM.Tests/bin/Debug/net10.0/playwright.ps1 install chromium
dotnet test OrangeHRM.Tests/OrangeHRM.Tests.csproj
```

The project automatically selects `OrangeHRM.Tests/RunSettings/test.runsettings`. Defaults are Chromium, a visible browser, sequential execution, and a 15-second assertion timeout.

Headless execution:

```powershell
dotnet test OrangeHRM.Tests/OrangeHRM.Tests.csproj -- Playwright.LaunchOptions.Headless=true
```

Run one scenario:

```powershell
dotnet test OrangeHRM.Tests/OrangeHRM.Tests.csproj --filter FullyQualifiedName~EmployeeWorkflowTests
```

Optional parallel execution:

```powershell
dotnet test OrangeHRM.Tests/OrangeHRM.Tests.csproj -- Playwright.LaunchOptions.Headless=true NUnit.NumberOfTestWorkers=2
```

Parallelism is between fixture classes; tests within a fixture run sequentially. Each test receives its own Playwright browser context. Sequential execution is the default because the shared demo has intermittent startup delays.

For debugging, use Test Explorer's Debug action with the same runsettings. The repository includes the VS Code C# Dev Kit runsettings selection; in Visual Studio, select the file under **Test > Configure Run Settings > Select Solution Wide runsettings File**. The browser is visible with the default settings.

## Configuration

Environment variables override the public demo defaults:

| Variable | Default |
| --- | --- |
| `ORANGEHRM_BASE_URL` | `https://opensource-demo.orangehrmlive.com/` |
| `ORANGEHRM_USERNAME` | `Admin` |
| `ORANGEHRM_PASSWORD` | `admin123` |

For example, to start directly at the login page in the current PowerShell session:

```powershell
$env:ORANGEHRM_BASE_URL = 'https://opensource-demo.orangehrmlive.com/web/index.php/auth/login'
```

If using a custom `PLAYWRIGHT_BROWSERS_PATH`, set it to the same absolute directory for browser installation and test execution. Otherwise Playwright uses its default browser cache.

## Design decisions

- **Page objects:** `Pages/` owns locators, interactions, and reusable assertions. Tests describe the scenario and expected data. Creation is verified on `EmployeeDetailsPage`, including the employee identity, rather than inferred from the creation form disappearing.
- **Fixtures:** `BaseTest` supplies page objects and failure reporting. `PimTestBase` adds login and employee cleanup. No additional service or repository layers are needed for this UI suite.
- **Synchronization:** Playwright actionability checks and retrying assertions handle UI readiness. Employee search also waits for the matching `GET /api/v2/pim/employees` response before checking the loader and results. Previous results may already be visible when another search starts; this wait distinguishes the new search from stale results. It couples this one page object to the endpoint, which must be updated if the application changes it. Assertions still check the UI.
- **Test data and cleanup:** `EmployeeData.CreateUnique()` supplies names and IDs with a random suffix. Records are registered for cleanup before Save, including attempted duplicate-ID records. Teardown targets both employee ID and the unchanged first/middle names to avoid deleting another record if IDs collide. It verifies absence and attempts the remaining cleanup candidates even if one fails. Cleanup failures are reported without replacing an existing test failure. Tests create employee records with Create Login Details disabled.

## Reports and failure artifacts

Each run using the supplied runsettings produces TRX and HTML reports in `OrangeHRM.Tests/TestResults/`. Open the HTML file in a browser for the run summary and failure details.

Failed tests also save one screenshot and one Playwright trace under:

```text
OrangeHRM.Tests/TestResults/artifacts/<UTC run timestamp>-<process ID>/<class>.<test>-<suffix>/
```

For PIM failures, the screenshot is captured before employee cleanup; the trace includes cleanup. Passing traces are discarded. Test output includes artifact paths and the page URL at failure. Cleanup and artifact-capture errors identify the affected test.

Artifacts are deliberately not registered with `AddTestAttachment`: VSTest would copy them into a second folder. Their paths are retained in TRX/HTML output instead. Share the reports together with the `artifacts` folder; logged absolute paths refer to the machine that ran the tests.

Open a trace with the generated Playwright CLI:

```powershell
pwsh OrangeHRM.Tests/bin/Debug/net10.0/playwright.ps1 show-trace "<full path to the trace.zip>"
```

`ResultsDirectory` is relative to the runsettings file; NUnit's `WorkDirectory` is relative to the built assembly. Both resolve to `OrangeHRM.Tests/TestResults` with the standard `bin/<configuration>/net10.0` layout. Results are ignored by Git and accumulate until removed; delete old results when they are no longer needed.

## Known limitation

The public demo sometimes remains blank and exceeds the 30-second navigation timeout, including at the direct login URL. This has occurred before test actions, in both sequential and parallel execution. The suite keeps these failures visible and saves diagnostics; it does not apply automatic retries. A later run may succeed. An interrupted process or unavailable application can also prevent teardown from completing.
