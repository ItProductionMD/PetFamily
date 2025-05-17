namespace PetFamily.Infrastructure.Contexts.ReadDbContext;

public static class ScaffoldToReadModels
{
    private const string command = "dotnet ef dbcontext scaffold " +
        "\"Host=localhost;Port=5432;Database=PetFamily;Username=postgres;Password=postgres\" " +
        "Npgsql.EntityFrameworkCore.PostgreSQL -o \"ReadDbContext\\Models\" " +
        "--context ReadDbContext " +
        "--no-onconfiguring --force\r\n";
}
