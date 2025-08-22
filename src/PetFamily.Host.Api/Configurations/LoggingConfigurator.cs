using Serilog;

namespace PetFamily.Host.Api.Configurations;

public static class LoggingConfigurator
{
    public static WebApplicationBuilder ConfigureLogger(this WebApplicationBuilder builder)
    {
        var seqConnection = builder.Configuration.GetConnectionString("Seq");
        if (string.IsNullOrWhiteSpace(seqConnection))
            throw new ArgumentNullException(nameof(seqConnection), "Seq connection string not found");

        Log.Logger = new LoggerConfiguration()
            //.MinimumLevel.Debug() 
            //.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            //.MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            //.Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.Seq(seqConnection)
            .CreateLogger();

        builder.Host.UseSerilog();

        return builder;

        return builder;
    }
}
