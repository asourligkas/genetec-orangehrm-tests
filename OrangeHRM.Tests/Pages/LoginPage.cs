using Microsoft.Playwright;
using OrangeHRM.Tests.Configuration;

namespace OrangeHRM.Tests.Pages;

public class LoginPage(IPage page) : BasePage(page)
{
    private ILocator UsernameInput => Page.GetByPlaceholder("Username", new() { Exact = true });
    private ILocator PasswordInput => Page.GetByPlaceholder("Password", new() { Exact = true });
    private ILocator LoginButton => Page.GetByRole(AriaRole.Button, new() { Name = "Login", Exact = true });

    public async Task OpenAsync()
    {
        await NavigateToAsync(TestSettings.BaseUrl);
    }

    public async Task LoginAsync(string username, string password)
    {
        await UsernameInput.FillAsync(username);
        await PasswordInput.FillAsync(password);
        await LoginButton.ClickAsync();
    }
}
