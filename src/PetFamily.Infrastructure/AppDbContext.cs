﻿
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PetFamily.Domain.PetAggregates.Entities;
using PetFamily.Domain.PetAggregates.Root;
using PetFamily.Domain.VolunteerAggregates.Root;
using static PetFamily.Domain.Shared.Constants;
using PetFamily.Infrastructure.Configurations;
using PetFamily.Domain.Shared;

namespace PetFamily.Infrastructure;

public class AppDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    DbSet<Volunteer> Volunteers { get; set; }
    //DbSet<Species> Speciesies { get; set; }
    //DbSet<Breed> Breeds { get; set; }
    //DbSet<Pet> Pets { get; set; }
    public AppDbContext(IConfiguration configuration)
    {
         _configuration=configuration;
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = _configuration.GetConnectionString(POSTGRE_CONNECTION_NAME);
        //var connectionString = "Host=localhost;Port=5432;Database=PetFamily;Username=postgres;Password=postgres";
        optionsBuilder
            .UseNpgsql(connectionString) 
            .UseSnakeCaseNamingConvention()
            .UseLoggerFactory(MyLoggerFactory.LoggerFactoryInstance)
            .EnableSensitiveDataLogging();
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder); 
    }
}