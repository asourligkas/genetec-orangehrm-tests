# OrangeHRM PIM UI tests

QA assessment using C#/.NET 10, NUnit, and Microsoft.Playwright.NUnit. C#/.NET was approved as an alternative to the originally requested Node.js framework.

Target: https://opensource-demo.orangehrmlive.com/

## Coverage

| Test | What it verifies |
| --- | --- |
| Administrator login | Valid credentials open the dashboard. |
| Employee workflow | Create, search by ID, open details, edit last name, Other ID, nationality and date of birth, then reopen and verify persistence. |
| Required names | Field-specific required errors clear as each name is supplied, allowing creation. |
| Duplicate employee ID | An existing ID prevents creation; correcting it creates the second employee and preserves the original. |
| Employee deletion | Cancel preserves the record; confirm removes it from a fresh search. |

The three additional PIM scenarios cover validation, identifier uniqueness, and deletion confirmation.

## Setup and execution

Prerequisites: .NET 10 SDK and PowerShell 7 (`pwsh`). From the repository root:

```powershell
dotnet restore OrangeHRM.Tests/OrangeHRM.Tests.csproj
dotnet build OrangeHRM.Tests/OrangeHRM.Tests.csproj --no-restore
pwsh OrangeHRM.Tests/bin/Debug/net10.0/playwright.ps1 install chromium
dotnet test OrangeHRM.Tests/OrangeHRM.Tests.csproj
```

The project automatically selects `OrangeHRM.Tests/RunSettings/test.runsettings`: Chromium, visible browser, sequential execution, and a 15-second assertion timeout.

```powershell
# Headless
dotnet test OrangeHRM.Tests/OrangeHRM.Tests.csproj -- Playwright.LaunchOptions.Headless=true
# One scenario
dotnet test OrangeHRM.Tests/OrangeHRM.Tests.csproj --filter FullyQualifiedName~EmployeeWorkflowTests
# Optional parallel run
dotnet test OrangeHRM.Tests/OrangeHRM.Tests.csproj -- Playwright.LaunchOptions.Headless=true NUnit.NumberOfTestWorkers=2
```

Parallelism is between fixture classes; tests within a fixture run sequentially. Each test gets its own browser context. Sequential execution is the default against the shared demo.

## Configuration

Environment variables override the public demo defaults:

| Variable | Default |
| --- | --- |
| `ORANGEHRM_BASE_URL` | `https://opensource-demo.orangehrmlive.com/` |
| `ORANGEHRM_USERNAME` | `Admin` |
| `ORANGEHRM_PASSWORD` | `admin123` |

To start directly at login, set `$env:ORANGEHRM_BASE_URL = 'https://opensource-demo.orangehrmlive.com/web/index.php/auth/login'` in PowerShell before running tests.

## Design decisions

- **Page objects:** `Pages/` owns locators, interactions, and reusable assertions. Tests describe scenarios and expected data. Creation is verified on the details page using the employee identity.
- **Fixtures:** `BaseTest` provides page objects and failure reporting. `PimTestBase` adds independent UI login and cleanup. This is sufficient for five tests.
- **Synchronization:** Playwright actionability checks and retrying assertions handle readiness. Search also waits for the matching `GET /api/v2/pim/employees` response before checking the loader and UI results, because previous results may still be visible. This endpoint dependency is isolated to `EmployeeListPage`.
- **Data and cleanup:** `EmployeeData.CreateUnique()` generates names and IDs. Records are registered before Save, including attempted duplicates. Teardown matches ID and unchanged first/middle names, verifies absence, and continues after individual cleanup failures. Cleanup errors are reported without replacing an existing test failure. Create Login Details stays disabled, so these are employee records.

## Reports and failure artifacts

TRX and HTML reports are generated in `OrangeHRM.Tests/TestResults/`. Open HTML in a browser for results and failure details. Failed tests save a screenshot and trace under:

```text
OrangeHRM.Tests/TestResults/artifacts/<UTC timestamp>-<process ID>/<class>.<test>-<suffix>/
```

For test-body failures, the PIM screenshot is taken before cleanup; the trace includes cleanup. Passing traces are discarded. Paths and the failure URL appear in test output and TRX/HTML. Files are not registered with `AddTestAttachment`, which would make VSTest copy them again. Share reports with the `artifacts` folder; logged absolute paths refer to the original machine.

```powershell
pwsh OrangeHRM.Tests/bin/Debug/net10.0/playwright.ps1 show-trace "<full path to the trace.zip>"
```

`ResultsDirectory` is relative to the runsettings file; NUnit's `WorkDirectory` is relative to the assembly. Both resolve to `OrangeHRM.Tests/TestResults` with the standard build layout. Results are ignored by Git; remove old runs when no longer needed.

## Startup limitation

The public demo can remain blank and exceed the 30-second navigation timeout, including at the direct login URL. Opening the login page retries once on a navigation timeout, with a message identifying the test. A second timeout fails normally. Credential submission is not retried, tests are not rerun, and assertion failures remain failures. Persistent unavailability still causes failures and can prevent cleanup.

## At larger scale

Evaluate reusable authenticated `storageState` for PIM setup while retaining the UI login test.
Consider API cleanup after validating authentication and record ownership, while retaining UI deletion coverage.
For five tests, independent UI setup and cleanup keep the suite straightforward and exercise the real workflow.
