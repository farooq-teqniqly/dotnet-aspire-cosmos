using Microsoft.AspNetCore.Mvc.Testing;

namespace WineApi.Tests
{
    /// <summary>
    /// Web application factory for integration testing of the WineApi application.
    /// </summary>
    public sealed class WineApiWebApplicationFactory : WebApplicationFactory<Program> { }
}
