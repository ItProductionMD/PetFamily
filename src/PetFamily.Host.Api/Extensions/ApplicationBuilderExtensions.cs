using Account.Application.DefaultSeeder;
using Authorization.Application.DefaultSeeder;
using PetFamily.SharedApplication.Abstractions;

namespace PetFamily.Host.Api.Extensions;

public static class ApplicationBuilderExtensions
{

    public async static Task SeedDefaultData(this IApplicationBuilder app)
    {
        await app.UseSeeder<RolesSeeder>();
        await app.UseSeeder<AdminSeeder>();
    }

    public async static Task UseSeeder<T> (this IApplicationBuilder app) where T: ISeeder
    {
        using var scope = app.ApplicationServices.CreateScope();
        var nameOfSeeder = nameof(T);
        var seeder = scope.ServiceProvider.GetRequiredService<T>();
        if (seeder is null)
            throw new Exception($"Seeder service:{nameOfSeeder} is null");

        await seeder.SeedAsync();
    }
}
