using Account.Infrastructure;
using Authorization.Infrastructure;
using FileStorage.Infrastructure;
using PetFamily.Discussions.Infrastructure;
using PetFamily.Framework.DependencyInjection;
using PetFamily.Framework.Middlewares;
using PetFamily.Host.Api.Configurations;
using PetFamily.Host.Api.Extensions;
using PetFamily.SharedInfrastructure.DependencyInjection;
using PetFamily.VolunteerRequests.Infrastructure;
using PetSpecies.Infrastructure;
using Serilog;
using Volunteers.Infrastructure;
using static PetFamily.Host.Api.Configurations.LoggingConfigurator;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<Program>();
builder.Configuration.AddUserSecrets<Program>();

//Add asp net core
builder.Services
    .AddEndpointsApiExplorer()
    .AddHttpContextAccessor()
    .AddControllers();

//Configure host
builder
    .ConfigureLogger()
    .ConfigureKestrel()
    .ConfigureSwagger();

//Add modules
builder.Services
    .AddVolunteerModule(builder.Configuration)
    .AddSpeciesModule(builder.Configuration)
    .AddDiscussionModule(builder.Configuration)
    .AddVolunteerRequestModule(builder.Configuration);

//Add building blocks
builder.Services
    .AddAuthorization(builder.Configuration)
    .AddAccount(builder.Configuration)
    .AddFileStorage(builder.Configuration);

//Add shared layers
builder.Services
    .AddSharedInfrastructure(builder.Configuration)
    .AddFramework(builder.Configuration);

var app = builder.Build();

app.UseSerilogRequestLogging()
   .UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger()
       .UseSwaggerUI();

    //await app.ApplyMigration();
    await app.SeedDefaultData();
}

app.UseHttpsRedirection()
   .UseAuthentication()
   .UseAuthorization();

app.MapControllers();
app.Run();

public partial class Program { }
