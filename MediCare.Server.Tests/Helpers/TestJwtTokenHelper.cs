using MediCare.Server.Helpers;
using Microsoft.Extensions.Configuration;

public static class TestJwtTokenHelper
{
    private static readonly IConfiguration configuration;

    static TestJwtTokenHelper()
    {
        configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
    }

    public static string GenerateTestToken(string id, string email, string name, string role)
    {
        var helper = new JwtTokenHelper(configuration);
        return helper.GenerateJwtToken(id, email, name, role);
    }
}
