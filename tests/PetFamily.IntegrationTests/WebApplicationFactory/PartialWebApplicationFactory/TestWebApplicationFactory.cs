using FileStorage.Public.Contracts;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Npgsql;
using PetFamily.Auth.Infrastructure.Contexts;
using PetFamily.Auth.Public.Contracts;
using PetFamily.SharedApplication.IUserContext;
using PetFamily.SharedInfrastructure.Constants;
using PetFamily.VolunteerRequests.Infrastructure.Contexts;
using PetSpecies.Infrastructure.Contexts;
using Respawn;
using Testcontainers.PostgreSql;
using Volunteers.Infrastructure.Contexts;

namespace PetFamily.IntegrationTests.WebApplicationFactory;

public partial class TestWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer =
        new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithDatabase("testpetfamilydb")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

    private Respawner _respawner = null!;
    private NpgsqlConnection _dbConnection = null!;
    public string ConnectionString { get; set; } = string.Empty;
    public Mock<IFileService> FileServiceMock = new Mock<IFileService>();
    public Mock<IUserContext> UserContextMock = new Mock<IUserContext>();
    public Mock<IUploadFileDtoValidator> FileValidator = new Mock<IUploadFileDtoValidator>();
    public Mock<IParticipantContract> ParticipantContractMock = new Mock<IParticipantContract>();
}
