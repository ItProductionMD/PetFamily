using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using PetFamily.IntegrationTests.WebApplicationFactory.Extensions;
using PetFamily.SharedApplication.Abstractions;
using PetFamily.SharedApplication.IUserContext;
using PetFamily.SharedInfrastructure.Shared.Dapper;
namespace PetFamily.IntegrationTests.WebApplicationFactory;

public partial class TestWebApplicationFactory
{
    private List<Type> _writeDbContextTypes = new();
  
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            _writeDbContextTypes =  services.ReplaceAllWriteDbContexts(ConnectionString);

            services.AddSingleton<IDbConnectionFactory>(_ => new NpgSqlConnectionFactory(ConnectionString));

            services.AddScoped(_ => ParticipantContractMock.Object);
            services.AddScoped(_ => FileServiceMock.Object);

            //TODO DELETE FROM TESTS AND APPLICATION LAYER  AND MOVE IT IN PRESENTATION LAYER
            services.AddScoped(_ => UserContextMock.Object);
        });
    }
}
