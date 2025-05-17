using Microsoft.EntityFrameworkCore;
using Moq;
using PetFamily.Application.Commands.FilesManagment.Dtos;
using PetFamily.Application.Commands.PetManagment.DeletePetImages;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.PetManagment.Entities;
using PetFamily.Domain.PetManagment.Root;
using PetFamily.Domain.Results;
using PetFamily.IntegrationTests.Seeds;
using PetFamily.IntegrationTests.TestData;

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
        await Seeder.Seed(seedSpecies, _dbContext);

        _seedVolunteer = new VolunteerTestBuilder()
            .WithPets(1, seedSpecies)
            .GetVolunteer();

        _seedVolunteer.Pets[0].AddImages([IMAGE_NAME]);

        await Seeder.Seed(_seedVolunteer, _dbContext);

        _seedPet = _seedVolunteer.Pets[0] ?? throw new Exception("seeded Pet is Null!");
    }

    [Fact]
    public async Task Should_DeletePetImage_Correctly()
    {
        //ARRANGE
        var sutCommand = new DeletePetImagesCommand(_seedVolunteer.Id, _seedPet.Id, [IMAGE_NAME]);
        SetupIFileService([IMAGE_NAME]);
        //ACT
        var sutResult = await _sut.Handle(sutCommand, CancellationToken.None);
        //ASSERT
        Assert.NotNull(sutResult);
        Assert.True(sutResult.IsSuccess);

        var updatedVolunteer = await _dbContext.Volunteers
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

        var updatedVolunteer = await _dbContext.Volunteers
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
        SetupIFileServiceWithError();
        //ACT
        var sutResult = await _sut.Handle(sutCommand, CancellationToken.None);
        //ASSERT
        Assert.NotNull(sutResult);
        Assert.True(sutResult.IsSuccess);

        var updatedVolunteer = await _dbContext.Volunteers
            .AsNoTracking()
            .Include(v => v.Pets)
            .SingleOrDefaultAsync();
        Assert.NotNull(updatedVolunteer);

        Assert.Empty(updatedVolunteer.Pets[0].Images);
    }

    private void SetupIFileService(List<string> fileServiceResponse)
    {
        var fileServiceResult = Result<List<string>>.Ok(fileServiceResponse);

        _factory.FileServiceMock.Reset();

        _factory.FileServiceMock
            .Setup(x => x.SoftDeleteFileListAsync(It.IsAny<List<AppFileDto>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fileServiceResult);
    }
    private void SetupIFileServiceWithError()
    {
        var fileServiceResult = Result<List<string>>.Fail(Error.InternalServerError("FileService doesn't answer"));

        _factory.FileServiceMock.Reset();

        _factory.FileServiceMock
            .Setup(x => x.SoftDeleteFileListAsync(It.IsAny<List<AppFileDto>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fileServiceResult);
    }
}
