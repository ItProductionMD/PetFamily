using FileStorage.Infrastructure;
using Microsoft.EntityFrameworkCore;
using PetFamily.Framework.Middlewares;
using PetFamily.SharedInfrastructure;
using PetSpecies.Infrastructure;
using Serilog;
using Volunteers.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddEndpointsApiExplorer();

builder.Services.InjectSharedInfrastructure(builder.Configuration);

builder.Services.InjectFileStorage(builder.Configuration);

builder.Services.InjectVolunteerModule(builder.Configuration);

builder.Services.InjectSpeciesModule(builder.Configuration);

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
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
