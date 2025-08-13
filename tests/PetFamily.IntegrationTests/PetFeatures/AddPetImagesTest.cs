using FileStorage.Public.Dtos;
using Microsoft.EntityFrameworkCore;
using Moq;
using PetFamily.IntegrationTests.IClassFixtures;
using PetFamily.IntegrationTests.Seeds;
using PetFamily.IntegrationTests.TestData;
using PetFamily.IntegrationTests.WebApplicationFactory;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetSpecies.Domain;
using Volunteers.Application.Commands.PetManagement.AddPetImages;
using Volunteers.Domain;


namespace PetFamily.IntegrationTests.PetFeatures;

public class AddPetImagesTest(TestWebApplicationFactory factory)
    : CommandHandlerTest<List<FileUploadResponse>, AddPetImagesCommand>(factory)
{
    public Volunteer SeedVolunteer { get; set; } = null!;
    public Species SeedSpecies { get; set; } = null!;
    public Breed Breed { get; set; } = null!;
    public const string OK_FILE_NAME = "ok_file.png";
    public const string ERROR_FILE_NAME = "error_file.png";

    [Fact]
    public async Task Should_add_one_pet_image_correctly()
    {
        var command = CreateCommandWithFileNames([OK_FILE_NAME], SeedVolunteer);

        var fileServiceOkResponse = CreateResponse(command);

        SetupIFileService(fileServiceOkResponse);
        //ACT
        var result = await _sut.Handle(command, CancellationToken.None);
        //ARRANGE
        var addedVolunteer = await _volunteerDbContext.Volunteers
            .AsNoTracking()
            .Include(v => v.Pets)
            .FirstOrDefaultAsync(v => v.Id == SeedVolunteer.Id);

        var addedPet = addedVolunteer!.Pets[0];

        Assert.True(result.IsSuccess);
        Assert.Equal(fileServiceOkResponse[0]!.StoredName, addedPet!.Images[0].Name);
    }

    [Fact]
    public async Task Should_add_one_pet_image_with_error()
    {
        var command = CreateCommandWithFileNames([ERROR_FILE_NAME], SeedVolunteer);

        List<FileUploadResponse>? fileServiceErrorResponse = null;

        SetupIFileService(fileServiceErrorResponse);
        //ACT
        var result = await _sut.Handle(command, CancellationToken.None);
        //ARRANGE
        var addedVolunteer = await _volunteerDbContext.Volunteers
            .AsNoTracking()
            .Include(v => v.Pets)
            .FirstOrDefaultAsync(v => v.Id == SeedVolunteer.Id);

        var addedPet = addedVolunteer!.Pets[0];

        Assert.True(result.IsFailure);
        Assert.Empty(addedPet!.Images);
    }

    [Fact]
    public async Task Should_add_one_pet_image_correctly_and_one_with_error()
    {
        var command = CreateCommandWithFileNames([OK_FILE_NAME, ERROR_FILE_NAME], SeedVolunteer);

        var response = CreateResponse(command);

        SetupIFileService(response);
        //ACT
        var result = await _sut.Handle(command, CancellationToken.None);
        //ARRANGE
        var addedVolunteer = await _volunteerDbContext.Volunteers
            .AsNoTracking()
            .Include(v => v.Pets)
            .FirstOrDefaultAsync(v => v.Id == SeedVolunteer.Id);

        var addedPet = addedVolunteer!.Pets[0];

        var expectedStoredName = command.UploadFileDtos
            .FirstOrDefault(c => c.OriginalName == OK_FILE_NAME)!.StoredName;

        Assert.True(result.IsSuccess);
        Assert.Single(addedPet!.Images);
        Assert.Equal(expectedStoredName, addedPet.Images[0].Name);
    }


    private AddPetImagesCommand CreateCommandWithFileNames(List<string> fileNames, Volunteer volunteer)
    {
        var uploadFileCommands = new List<UploadFileDto>();
        foreach (var name in fileNames)
        {
            uploadFileCommands.Add(new(
            name,
            string.Concat(Guid.NewGuid(), ".png"),
            ".png",
            "image/png",
            4000,
            new MemoryStream(new byte[] { 0x89, 0x50 }),
            "FolderName"));
        }
        return new(
            volunteer.Id,
            volunteer.Pets[0].Id,
            uploadFileCommands);
    }

    private static List<FileUploadResponse> CreateResponse(AddPetImagesCommand command)
    {
        var response = new List<FileUploadResponse>();
        foreach (var file in command.UploadFileDtos)
        {
            response.Add(new(
                file.OriginalName,
                file.StoredName,
                file.OriginalName == OK_FILE_NAME ? true : false,
                string.Empty));
        }
        return response;
    }

    private void SetupIFileService(List<FileUploadResponse>? response)
    {
        var fileUploaderResult = response == null
            ? Result<List<FileUploadResponse>>.Fail(Error.InternalServerError("Error handle file!"))
            : Result<List<FileUploadResponse>>.Ok(response);

        _factory.FileServiceMock.Reset();

        _factory.FileServiceMock
            .Setup(x => x.UploadFilesAsync(It.IsAny<List<UploadFileDto>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => fileUploaderResult);
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        SeedSpecies = new SpeciesTestBuilder()
            .WithBreeds(["breedOne"]).Species;
        await DbContextSeedExtensions.SeedAsync(_speciesDbContext, SeedSpecies);

        SeedVolunteer = new VolunteerTestBuilder()
            .WithPets(1, SeedSpecies).Volunteer;

        await DbContextSeedExtensions.SeedAsync(_volunteerDbContext, SeedVolunteer);
    }
}
