using Microsoft.EntityFrameworkCore;
using PetFamily.API.Extensions;
using PetFamily.API.Middlewares;
using PetFamily.Application;
using PetFamily.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<Program>();

builder.Configuration.AddUserSecrets<Program>();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Seq(builder.Configuration.GetConnectionString("Seq") ?? throw new ArgumentNullException("Seq"))
    .CreateLogger();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services
    .AddApplication()
    .AddInfrastructure();

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
   // await app.ApplyMigration();    
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
