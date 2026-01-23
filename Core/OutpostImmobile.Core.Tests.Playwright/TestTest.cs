using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace OutpostImmobile.Core.Tests.Playwright;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public partial class TestTest : PageTest
{
    [Test]
    public async Task HasTitle()
    {
        await Page.GotoAsync("https://playwright.dev");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(MyRegex());
    }

    [Test]
    public async Task GetStartedLink()
    {
        await Page.GotoAsync("https://playwright.dev");

        // Click the get started link.
        await Page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Get started" }).ClickAsync();

        // Expects page to have a heading with the name of Installation.
        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Installation" })).ToBeVisibleAsync();
    }

    [GeneratedRegex("Playwright")]
    private static partial Regex MyRegex();
}