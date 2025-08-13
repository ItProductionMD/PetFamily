using FileStorage.Public.Dtos;
using Microsoft.EntityFrameworkCore;
using Moq;
using PetFamily.IntegrationTests.Fixtures;
using PetFamily.IntegrationTests.Seeds;
using PetFamily.IntegrationTests.TestData;
using PetFamily.IntegrationTests.WebApplicationFactory;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using Volunteers.Application.Commands.PetManagement.DeletePetImages;
using Volunteers.Domain;

namespace PetFamily.IntegrationTests.PetFeatures;

public class DeletePetImagesTest(TestWebApplicationFactory factory)
    : CommandHandlerTest<DeletePetImagesCommand>(factory)
{
    private Volunteer _seedVolunteer { get; set; } = null!;
    private Pet _seedPet { get; set; } = null!;
    public const string IMAGE_NAME = "test_image.png";

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        var seedSpecies = new SpeciesTestBuilder()
            .WithBreeds(["testBreed"])
            .GetSpecies();
        await DbContextSeedExtensions.SeedAsync(_speciesDbContext, seedSpecies);

        _seedVolunteer = new VolunteerTestBuilder()
            .WithPets(1, seedSpecies)
            .GetVolunteer();

        _seedVolunteer.Pets[0].AddImages([IMAGE_NAME]);

        await DbContextSeedExtensions.SeedAsync(_volunteerDbContext, _seedVolunteer);

        _seedPet = _seedVolunteer.Pets[0] ?? throw new Exception("seeded Pet is Null!");
    }

    [Fact]
    public async Task Should_DeletePetImage_Correctly()
    {
        //ARRANGE
        var sutCommand = new DeletePetImagesCommand(_seedVolunteer.Id, _seedPet.Id, [IMAGE_NAME]);
        SetupIFileServiceToOk([IMAGE_NAME]);
        //ACT
        var sutResult = await _sut.Handle(sutCommand, CancellationToken.None);
        //ASSERT
        Assert.NotNull(sutResult);
        Assert.True(sutResult.IsSuccess);

        var updatedVolunteer = await _volunteerDbContext.Volunteers
            .AsNoTracking()
            .Include(v => v.Pets)
            .SingleOrDefaultAsync();
        Assert.NotNull(updatedVolunteer);

        Assert.Empty(updatedVolunteer.Pets[0].Images);
    }

    [Fact]
    public async Task Should_ReturnNotFoundError_When_DeletingNonExistentPetImage()
    {
        //ARRANGE
        var sutCommand = new DeletePetImagesCommand(_seedVolunteer.Id, _seedPet.Id, ["non_existent.png"]);
        //ACT
        var sutResult = await _sut.Handle(sutCommand, CancellationToken.None);
        //ASSERT
        Assert.NotNull(sutResult);
        Assert.True(sutResult.IsFailure);
        Assert.Equal(ErrorType.NotFound, sutResult.Error.Type);

        var updatedVolunteer = await _volunteerDbContext.Volunteers
            .AsNoTracking()
            .Include(v => v.Pets)
            .SingleOrDefaultAsync();
        Assert.NotNull(updatedVolunteer);

        Assert.Equal(IMAGE_NAME, updatedVolunteer.Pets[0].Images[0].Name);
    }

    [Fact]
    public async Task Should_DeletePetImage_EvenWhenFileServiceFails()
    {
        //ARRANGE
        var sutCommand = new DeletePetImagesCommand(_seedVolunteer.Id, _seedPet.Id, [IMAGE_NAME]);
        SetupIFileServiceToError();
        //ACT
        var sutResult = await _sut.Handle(sutCommand, CancellationToken.None);
        //ASSERT
        Assert.NotNull(sutResult);
        Assert.True(sutResult.IsSuccess);

        var updatedVolunteer = await _volunteerDbContext.Volunteers
            .AsNoTracking()
            .Include(v => v.Pets)
            .SingleOrDefaultAsync();
        Assert.NotNull(updatedVolunteer);

        Assert.Empty(updatedVolunteer.Pets[0].Images);
    }

    private void SetupIFileServiceToOk(List<string> fileServiceResponse)
    {
        var fileRemover = Result<List<string>>.Ok(fileServiceResponse);

        _factory.FileServiceMock.Reset();

        _factory.FileServiceMock
            .Setup(x => x.DeleteFilesUsingMessageQueue(It.IsAny<List<FileDto>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }
    private void SetupIFileServiceToError()
    {
        var fileRemover = Result<List<string>>
            .Fail(Error.InternalServerError("FileService doesn't answer"));

        _factory.FileServiceMock.Reset();

        _factory.FileServiceMock
            .Setup(x => x.DeleteFilesUsingMessageQueue(It.IsAny<List<FileDto>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }
}
