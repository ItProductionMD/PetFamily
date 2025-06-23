using FileStorage.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PetFamily.Auth.Application.DefaultSeeder;
using PetFamily.Auth.Infrastructure.AuthInjector;
using PetFamily.Framework.Middlewares;
using PetFamily.SharedInfrastructure;
using PetSpecies.Infrastructure;
using Serilog;
using Volunteers.Infrastructure;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

var c = builder.Configuration;

builder.Services.AddOptions<Program>();

builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddControllers();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Seq(
        builder.Configuration.GetConnectionString("Seq")
        ?? throw new ArgumentNullException("Seq configuration wasn't found!"))
    .CreateLogger();
var logger = Log.Logger;

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 100 * 1024 * 1024; // 100MB
});

builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.InjectSharedInfrastructure(builder.Configuration);

builder.Services.InjectFileStorage(builder.Configuration);

builder.Services.InjectVolunteerModule(builder.Configuration);

builder.Services.InjectSpeciesModule(builder.Configuration);

builder.Services.InjectPetFamilyAuth(builder.Configuration);

builder.Services.AddSwaggerGen();

builder.Services.AddSerilog();

var app = builder.Build();


app.UseSerilogRequestLogging();

app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    //await app.ApplyMigration();    

    using var scope = app.Services.CreateScope();

    var rolesSeeder = scope.ServiceProvider.GetRequiredService<RolesSeeder>();
    var adminSeeder = scope.ServiceProvider.GetRequiredService<AdminSeeder>();

    await rolesSeeder.SeedAsync();
    await adminSeeder.SeedAsync();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
