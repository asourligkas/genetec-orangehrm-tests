using Microsoft.Playwright;

namespace OrangeHRM.Tests.Pages;

public class LoginPage(IPage page)
{
    private ILocator UsernameInput => page.GetByPlaceholder("Username", new() { Exact = true });
    private ILocator PasswordInput => page.GetByPlaceholder("Password", new() { Exact = true });
    private ILocator LoginButton => page.GetByRole(AriaRole.Button, new() { Name = "Login", Exact = true });

    public async Task NavigateAsync(string baseUrl)
    {
        await page.GotoAsync(baseUrl);
    }

    public async Task LoginAsync(string username, string password)
    {
        await UsernameInput.FillAsync(username);
        await PasswordInput.FillAsync(password);
        await LoginButton.ClickAsync();
    }
}
