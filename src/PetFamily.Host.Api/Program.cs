using Authorization.Application.DefaultSeeder;
using FileStorage.Infrastructure;
using Account.Application.DefaultSeeder;
using Account.Presentation;
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
using Account.Infrastructure;
using PetFamily.Framework.HTTPContext.User;
using PetFamily.SharedInfrastructure.HttpContext;
using PetFamily.Framework.HTTPContext.Cookie;
using Authorization.Infrastructure;


DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<Program>();
builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddControllers();

builder
    .ConfigureLogger()
    .ConfigureKestrel()
    .ConfigureSwagger();

builder.Services.Configure<RefreshTokenCookieOptions>(builder.Configuration.GetSection(RefreshTokenCookieOptions.SECTION_NAME));

builder.Services
    .AddScoped<IUserContext, UserContext>()
    .AddScoped<ICookieService, CookieService>();
    

builder.Services
    .AddEndpointsApiExplorer()
    .AddHttpContextAccessor();


builder.Services
    .InjectVolunteerModule(builder.Configuration)
    .InjectSpeciesModule(builder.Configuration)
    .InjectDiscussionModule(builder.Configuration)
    .InjectVolunteerRequestModule(builder.Configuration);

builder.Services
    .InjectAuthorization(builder.Configuration)
    .InjectSharedInfrastructure(builder.Configuration)
    .InjectFileStorage(builder.Configuration)
    .InjectPermissionPoliciesAuthorization()
    
    .InjectUserAccount(builder.Configuration);


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

    await rolesSeeder.SeedAsync();
    await adminSeeder.SeedAsync();
}

app
    .UseHttpsRedirection()
    .UseAuthentication()
    .UseAuthorization();

app.MapControllers();
app.Run();

public partial class Program { }
