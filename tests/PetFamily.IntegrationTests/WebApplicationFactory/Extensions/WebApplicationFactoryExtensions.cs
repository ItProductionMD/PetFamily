using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace PetFamily.IntegrationTests.WebApplicationFactory.Extensions;

public static class WebApplicationFactoryExtensions
{
    /// <summary>
    ///  Find all contexts in the DIContainer witch contain "WriteDbContext" ,then remove them and 
    ///  creatin new connection string
    /// </summary>
    /// <param name="services"></param>
    /// <param name="connectionString"></param>
    public static List<Type> ReplaceAllWriteDbContexts(this IServiceCollection services, string connectionString)
    {
        var writeDbContexts = services
            .Where(s => typeof(DbContext).IsAssignableFrom(s.ServiceType) &&
                        s.ServiceType.Name.EndsWith("WriteDbContext"))
            .Select(s => s.ServiceType)
            .Distinct()
            .ToList();

        foreach (var dbContextType in writeDbContexts)
        {
            var descriptor = services.SingleOrDefault(s => s.ServiceType == dbContextType);
            if (descriptor != null)
                services.Remove(descriptor);

            services.AddScoped(dbContextType, _ =>
                Activator.CreateInstance(dbContextType, connectionString)!);
        }
        
        return writeDbContexts;
    }
}
