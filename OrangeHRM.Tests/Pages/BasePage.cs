using Microsoft.Playwright;

namespace OrangeHRM.Tests.Pages;

public abstract class BasePage(IPage page)
{
    protected IPage Page { get; } = page;

    protected async Task NavigateToAsync(string url)
    {
        await Page.GotoAsync(url);
    }
}
