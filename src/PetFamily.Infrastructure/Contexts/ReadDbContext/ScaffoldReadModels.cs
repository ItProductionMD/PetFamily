using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Infrastructure.Constants;
using PetFamily.Infrastructure.Contexts;
using System.Diagnostics;
using System.IO;

namespace PetFamily.Infrastructure.Contexts.ReadDbContext;

public static class ScaffoldToReadModels
{
    private const string command = "dotnet ef dbcontext scaffold " +
        "\"Host=localhost;Port=5432;Database=PetFamily;Username=postgres;Password=postgres\" " +
        "Npgsql.EntityFrameworkCore.PostgreSQL -o \"ReadDbContext\\Models\" " +
        "--context ReadDbContext " +
        "--no-onconfiguring --force\r\n"; 
}
