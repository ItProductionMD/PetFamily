using PetFamily.IntegrationTests.IClassFixtures;
using PetFamily.IntegrationTests.Seeds;
using PetFamily.IntegrationTests.TestData;
using PetFamily.IntegrationTests.WebApplicationFactory;
using Volunteers.Application.Queries.GetPets;
using Volunteers.Application.Queries.GetPets.ForFilter;
using Volunteers.Domain.Enums;

namespace PetFamily.IntegrationTests.PetFeatures;

public class GetPetsQueryTest(TestWebApplicationFactory factory) 
    : QueryHandlerTest<GetPetsResponse, GetPetsQuery>(factory)
{
    [Fact]
    public async Task Should_GetPets_Successfully()
    {
        //ARRANGE
        var pageNumber = 1;
        var pageSize = 10;
        var volunteersCount = 10;
        var petsInVolunteersCount = 15;
        var totalPetsCount = volunteersCount * petsInVolunteersCount;

        var species = SpeciesTestBuilderNew.Build();
        await DbContextSeedExtensions.SeedRangeAsync(_speciesDbContext, species);

        var volunteers = VolunteerTestBuilder.Build(volunteersCount, petsInVolunteersCount, species);
        await DbContextSeedExtensions.SeedRangeAsync(_volunteerDbContext, volunteers);

        var command = new GetPetsQuery(pageNumber, pageSize);
        //ACT
        var result = await _sut.Handle(command, CancellationToken.None);
        //ASSERT
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(totalPetsCount, result.Data.TotalCount);
    }

    [Fact]
    public async Task Should_GetPets_Successfully_WithFilter_BySpeciesId()
    {
        //ARRANGE
        var pageNumber = 1;
        var volunteersCount = 10;
        var petsInVolunteersCount = 15;
        var totalPetsCount = volunteersCount * petsInVolunteersCount;
        var pageSize = totalPetsCount;

        var species = SpeciesTestBuilderNew.Build();
        await DbContextSeedExtensions.SeedRangeAsync(_speciesDbContext, species);

        var volunteers = VolunteerTestBuilder.Build(volunteersCount, petsInVolunteersCount, species);
        await DbContextSeedExtensions.SeedRangeAsync(_volunteerDbContext, volunteers);

        var petsFilter = new PetsFilter()
        {
            SpeciesIds = [species[0].Id]
        };
        var command = new GetPetsQuery(pageNumber, pageSize, petsFilter);
        //ACT
        var result = await _sut.Handle(command, CancellationToken.None);
        //ASSERT
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);

        var expectedSpeciesCount = volunteers
            .SelectMany(v => v.Pets)
            .Count(p => p.PetType.SpeciesId == species[0].Id);

        Assert.Equal(expectedSpeciesCount, result.Data.TotalCount);

        Assert.All(result.Data.Pets, p => Assert.Equal(species[0].Name, p.SpeciesName));
    }

    [Fact]
    public async Task Should_GetPets_Successfully_WithFilter_ByBreedsId()
    {
        //ARRANGE
        var pageNumber = 1;
        var volunteersCount = 10;
        var petsInVolunteersCount = 15;
        var totalPetsCount = volunteersCount * petsInVolunteersCount;
        var pageSize = totalPetsCount;

        var species = SpeciesTestBuilderNew.Build();
        await DbContextSeedExtensions.SeedRangeAsync(_speciesDbContext, species);

        var volunteers = VolunteerTestBuilder.Build(volunteersCount, petsInVolunteersCount, species);
        await DbContextSeedExtensions.SeedRangeAsync(_volunteerDbContext, volunteers);

        var filterBreedId = species[0].Breeds[0].Id;

        var petsFilter = new PetsFilter()
        {
            BreedIds = [filterBreedId]
        };
        var command = new GetPetsQuery(pageNumber, pageSize, petsFilter);
        //ACT
        var result = await _sut.Handle(command, CancellationToken.None);
        //ASSERT
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);

        var expectedSpeciesCount = volunteers
            .SelectMany(v => v.Pets)
            .Count(p => p.PetType.BreedId == filterBreedId);

        Assert.Equal(expectedSpeciesCount, result.Data.TotalCount);

        Assert.All(result.Data.Pets, p => Assert.Equal(species[0].Breeds[0].Name, p.BreedName));
    }

    [Fact]
    public async Task Should_GetPets_Successfully_WithFilter_ByAge()
    {
        //ARRANGE
        var pageNumber = 1;
        var volunteersCount = 10;
        var petsInVolunteersCount = 15;
        var totalPetsCount = volunteersCount * petsInVolunteersCount;
        var pageSize = totalPetsCount;
        var minAgeInMonths = 1;
        var maxAgeInMonths = 2;

        var species = SpeciesTestBuilderNew.Build();
        await DbContextSeedExtensions.SeedRangeAsync(_speciesDbContext, species);

        var volunteers = VolunteerTestBuilder.Build(volunteersCount, petsInVolunteersCount, species);
        await DbContextSeedExtensions.SeedRangeAsync(_volunteerDbContext, volunteers);

        var filterBreedId = species[0].Breeds[0].Id;

        var petsFilter = new PetsFilter()
        {
            MinAgeInMonth = minAgeInMonths,
            MaxAgeInMonth = maxAgeInMonths
        };

        var minDateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddMonths(-maxAgeInMonths));
        var maxDateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddMonths(-minAgeInMonths));

        var command = new GetPetsQuery(pageNumber, pageSize, petsFilter);
        //ACT
        var result = await _sut.Handle(command, CancellationToken.None);
        //ASSERT
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);


        var expectedSpeciesCount = volunteers
            .SelectMany(v => v.Pets)
            .Count(p => p.DateOfBirth < maxDateOfBirth && p.DateOfBirth > minDateOfBirth);

        Assert.Equal(expectedSpeciesCount, result.Data.TotalCount);

        Assert.All(
            result.Data.Pets,
            p => Assert.True(p.AgeInMonths >= minAgeInMonths && p.AgeInMonths <= maxAgeInMonths)
            );
    }

    [Fact]
    public async Task Should_GetPets_Successfully_WithFilter_ByColor()
    {
        //ARRANGE
        var pageNumber = 1;
        var volunteersCount = 10;
        var petsInVolunteersCount = 15;
        var totalPetsCount = volunteersCount * petsInVolunteersCount;
        var pageSize = totalPetsCount;
        var color = "red";

        var species = SpeciesTestBuilderNew.Build();
        await DbContextSeedExtensions.SeedRangeAsync(_speciesDbContext, species);

        var volunteers = VolunteerTestBuilder.Build(volunteersCount, petsInVolunteersCount, species);
        await DbContextSeedExtensions.SeedRangeAsync(_volunteerDbContext, volunteers);

        var petsFilter = new PetsFilter()
        {
            Color = color
        };
        var command = new GetPetsQuery(pageNumber, pageSize, petsFilter);
        //ACT
        var result = await _sut.Handle(command, CancellationToken.None);
        //ASSERT
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);

        var expectedSpeciesCount = volunteers
            .SelectMany(v => v.Pets)
            .Count(p => p.Color == color);

        Assert.Equal(expectedSpeciesCount, result.Data.TotalCount);

        Assert.All(
            result.Data.Pets,
            p => Assert.True(p.Color == color));
    }

    [Fact]
    public async Task Should_GetPets_Successfully_WithFilter_ByHelpStatus()
    {
        //ARRANGE
        var pageNumber = 1;
        var volunteersCount = 10;
        var petsInVolunteersCount = 15;
        var totalPetsCount = volunteersCount * petsInVolunteersCount;
        var pageSize = totalPetsCount;
        var status = HelpStatus.Helped;

        var species = SpeciesTestBuilderNew.Build();
        await DbContextSeedExtensions.SeedRangeAsync(_speciesDbContext, species);

        var volunteers = VolunteerTestBuilder.Build(volunteersCount, petsInVolunteersCount, species);

        var pet = volunteers.SelectMany(v => v.Pets).First();

        pet.ChangePetStatus(HelpStatus.Helped);

        await DbContextSeedExtensions.SeedRangeAsync(_volunteerDbContext, volunteers);

        var petsFilter = new PetsFilter()
        {
            HelpStatuses = [status]
        };
        var command = new GetPetsQuery(pageNumber, pageSize, petsFilter);
        //ACT
        var result = await _sut.Handle(command, CancellationToken.None);
        //ASSERT
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);

        var expectedSpeciesCount = volunteers
            .SelectMany(v => v.Pets)
            .Count(p => p.HelpStatus == status);

        Assert.Equal(expectedSpeciesCount, result.Data.TotalCount);

        Assert.All(
            result.Data.Pets,
            p => Assert.True(p.StatusForHelp == status.ToString()));
    }
}
