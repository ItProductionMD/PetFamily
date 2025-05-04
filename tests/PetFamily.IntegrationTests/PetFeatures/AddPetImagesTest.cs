using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Commands.FilesManagment.Commands;
using PetFamily.Application.Commands.FilesManagment.Dtos;
using PetFamily.Application.Commands.PetManagment.AddPetImages;
using PetFamily.Application.IRepositories;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.PetManagment.Root;
using PetFamily.Domain.PetManagment.ValueObjects;
using PetFamily.Domain.PetTypeManagment.Entities;
using PetFamily.Domain.PetTypeManagment.Root;
using PetFamily.Domain.Results;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Infrastructure.Contexts;
using PetFamily.Infrastructure.Repositories.Read;
using PetFamily.IntegrationTests.Seeds;
using PetFamily.IntegrationTests.TestData;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace PetFamily.IntegrationTests.PetFeatures;

public class AddPetImagesTest(TestWebApplicationFactory factory)
    : CommandHandlerTest<List<FileUploadResponse>, AddPetImagesCommand>(factory)
{
    public Volunteer SeedVolunteer { get; set; } = null!;
    public Species SeedSpecies { get;set; } = null!;
    public Breed Breed {  get; set; } = null!;  
    public const string OK_FILE_NAME = "ok_file.png";
    public const string ERROR_FILE_NAME = "error_file.png";

    [Fact]
    public async Task Should_add_one_pet_image_correctly()
    {
        var command = CreateCommandWithFileNames([OK_FILE_NAME],SeedVolunteer);

        var fileServiceOkResponse = CreateResponse(command);

        SetupIFileService(fileServiceOkResponse);
        //ACT
        var result = await _sut.Handle(command, CancellationToken.None);
        //ARRANGE
        var addedVolunteer = await _dbContext.Volunteers
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
        var command = CreateCommandWithFileNames([ERROR_FILE_NAME],SeedVolunteer);

        List<FileUploadResponse>? fileServiceErrorResponse = null;

        SetupIFileService(fileServiceErrorResponse);
        //ACT
        var result = await _sut.Handle(command, CancellationToken.None);
        //ARRANGE
        var addedVolunteer = await _dbContext.Volunteers
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
        var command = CreateCommandWithFileNames([OK_FILE_NAME,ERROR_FILE_NAME], SeedVolunteer);

        var response = CreateResponse(command);

        SetupIFileService(response);
        //ACT
        var result = await _sut.Handle(command, CancellationToken.None);
        //ARRANGE
        var addedVolunteer = await _dbContext.Volunteers
            .AsNoTracking()
            .Include(v => v.Pets)
            .FirstOrDefaultAsync(v => v.Id == SeedVolunteer.Id);

        var addedPet = addedVolunteer!.Pets[0];

        var expectedStoredName = command.UploadFileCommands
            .FirstOrDefault(c => c.OriginalName == OK_FILE_NAME)!.StoredName;

        Assert.True(result.IsSuccess);
        Assert.Single(addedPet!.Images);
        Assert.Equal(expectedStoredName, addedPet.Images[0].Name);
    }

   
    private AddPetImagesCommand CreateCommandWithFileNames(List<string> fileNames,Volunteer volunteer)
    {
        var uploadFileCommands = new List<UploadFileCommand>();
        foreach (var name in fileNames) 
        {
            uploadFileCommands.Add(new(
            name,
            "image/png",
            4000,
            new MemoryStream(new byte[] { 0x89, 0x50 })));
        }
        return new(
            volunteer.Id,
            volunteer.Pets[0].Id,
            uploadFileCommands);
    }

    private static List<FileUploadResponse> CreateResponse(AddPetImagesCommand command)
    {
        var response = new List<FileUploadResponse>();
        foreach (var file in command.UploadFileCommands)
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
        var error = Error
           .InternalServerError($"ProcessFile: {ERROR_FILE_NAME} unexpected error!");
        if (response == null)
        {
            _factory.FileServiceMock.Reset();

            _factory.FileServiceMock
                .Setup(x => x.UploadFileListAsync(It.IsAny<List<AppFileDto>>(), CancellationToken.None))
                .ReturnsAsync(Result<List<FileUploadResponse>>.Fail(error));
        }
        else
        {
            _factory.FileServiceMock.Reset();

            _factory.FileServiceMock
                .Setup(x => x.UploadFileListAsync(It.IsAny<List<AppFileDto>>(), CancellationToken.None))
                .ReturnsAsync(Result<List<FileUploadResponse>>.Ok(response));
        }
    }

    public  override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        SeedSpecies = new SpeciesTestBuilder()
            .WithBreeds(["breedOne"]).Species;
        await Seeder.Seed(SeedSpecies, _dbContext);  

        SeedVolunteer = new VolunteerTestBuilder()
            .WithPets(1,SeedSpecies).Volunteer;
        await Seeder.Seed(SeedVolunteer, _dbContext);
    }
}
