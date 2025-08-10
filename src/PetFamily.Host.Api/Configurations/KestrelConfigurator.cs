using PetFamily.Host.Api.Configurations;

namespace PetFamily.Host.Api.Configurations;

public static class KestrelConfigurator
{
    public static WebApplicationBuilder ConfigureKestrel(this WebApplicationBuilder builder)
    {
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10 MB
            //options.Limits.MaxRequestHeadersTotalSize = 16 * 1024; // 16 KB
        });
        return builder;
    }
}