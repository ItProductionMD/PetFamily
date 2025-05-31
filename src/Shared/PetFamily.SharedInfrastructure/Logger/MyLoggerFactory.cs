using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace PetFamily.SharedInfrastructure.Shared.Logger;
public static class MyLoggerFactory
{
    public static readonly ILoggerFactory LoggerFactoryInstance = LoggerFactory.Create(builder =>
    {
        builder
            .AddConsole()
            .AddFilter((category, level) =>
                category == DbLoggerCategory.Database.Command.Name
                && level == LogLevel.Information);
    });

}
