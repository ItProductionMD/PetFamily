using FileStorage.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using PetFamily.Auth.Application.DefaultSeeder;
using PetFamily.Auth.Infrastructure.AuthInjector;
using PetFamily.Discussions.Infrastructure;
using PetFamily.Framework.Middlewares;
using PetFamily.Framework.SharedAuthorization;
using PetFamily.Host.Api.Configurations;
using PetFamily.SharedInfrastructure;
using PetFamily.VolunteerRequests.Infrastructure;
using PetSpecies.Infrastructure;
using Serilog;
using Volunteers.Infrastructure;
using static PetFamily.Host.Api.Configurations.LoggingConfigurator;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<Program>();
builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddControllers();

builder
    .ConfigureLogger()
    .ConfigureKestrel()
    .ConfigureSwagger();

builder.Services
    .AddEndpointsApiExplorer()
    .AddHttpContextAccessor();

builder.Services
    .InjectSharedInfrastructure(builder.Configuration)
    .InjectFileStorage(builder.Configuration)
    .InjectAuth(builder.Configuration)
    .InjectPermissionPoliciesAuthorization();

builder.Services
    .InjectVolunteerModule(builder.Configuration)
    .InjectSpeciesModule(builder.Configuration)
    .InjectDiscussionModule(builder.Configuration)
    .InjectVolunteerRequestModule(builder.Configuration);


builder.Services
    .AddSwaggerGen()
    .AddSerilog();

var app = builder.Build();

app.
    UseSerilogRequestLogging()
   .UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app
        .UseSwagger()
        .UseSwaggerUI();
    //await app.ApplyMigration();    

    using var scope = app.Services.CreateScope();

    var rolesSeeder = scope.ServiceProvider.GetRequiredService<RolesSeeder>();
    var adminSeeder = scope.ServiceProvider.GetRequiredService<AdminSeeder>();

    //await rolesSeeder.SeedAsync();
    //await adminSeeder.SeedAsync();
}

app
    .UseHttpsRedirection()
    .UseAuthentication()
    .UseAuthorization();

app.MapControllers();
app.Run();

public partial class Program { }
