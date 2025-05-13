using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PetFamily.Infrastructure.Contexts;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<WriteDbContext>(options =>
            options.UseNpgsql("Host=localhost;Database=your_db;Username=postgres;Password=yourpassword"));
    })
    .Build();

using var scope = host.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<WriteDbContext>();

Console.WriteLine("Добавляю тестовые данные...");

// Пример данных
if (!await db.Pets.AnyAsync())
{
    db.Pets.Add(new Pet
    {
        Id = Guid.NewGuid(),
        Name = "Тестовый кот",
        DateOfBirth = DateTime.UtcNow.AddMonths(-8),
        Color = "Белый",
        HelpStatus = HelpStatus.NeedsHelp,
        // и другие нужные свойства
    });

    await db.SaveChangesAsync();
    Console.WriteLine("✅ Данные добавлены.");
}
else
{
    Console.WriteLine("⚠️ Данные уже существуют.");
}
