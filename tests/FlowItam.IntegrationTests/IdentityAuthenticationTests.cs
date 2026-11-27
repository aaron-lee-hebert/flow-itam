using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace FlowItam.IntegrationTests;

public class IdentityAuthenticationTests
{
    [Test]
    public async Task Identity_pages_and_health_are_public_while_application_pages_require_authentication()
    {
        using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder.UseEnvironment("Development"));
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var registerResponse = await client.GetAsync("/Identity/Account/Register");
        var registerHtml = await registerResponse.Content.ReadAsStringAsync();
        var loginResponse = await client.GetAsync("/Identity/Account/Login");
        var loginHtml = await loginResponse.Content.ReadAsStringAsync();
        var bootstrapResponse = await client.GetAsync("/lib/bootstrap/dist/css/bootstrap.min.css");
        var homeResponse = await client.GetAsync("/");
        var healthResponse = await client.GetAsync("/health");

        Assert.That(registerResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(registerHtml, Does.Contain("Organization name"));
        Assert.That(loginResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(loginHtml, Does.Contain("/lib/bootstrap/dist/css/bootstrap.min.css"));
        Assert.That(loginHtml, Does.Contain("Flow ITAM"));
        Assert.That(bootstrapResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(homeResponse.StatusCode, Is.EqualTo(HttpStatusCode.Redirect));
        Assert.That(homeResponse.Headers.Location?.OriginalString, Does.Contain("/Identity/Account/Login"));
        Assert.That(
            healthResponse.StatusCode,
            Is.AnyOf(HttpStatusCode.OK, HttpStatusCode.ServiceUnavailable));
    }
}