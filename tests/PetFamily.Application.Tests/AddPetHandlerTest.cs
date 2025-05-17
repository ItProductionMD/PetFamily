using Microsoft.Extensions.Logging;
using Moq;
using PetFamily.Application.Commands.PetManagment.AddPet;
using PetFamily.Application.IRepositories;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.PetManagment.Root;
using PetFamily.Domain.PetManagment.ValueObjects;
using PetFamily.Domain.PetTypeManagment.Entities;
using PetFamily.Domain.Results;
using DomainSpecies = PetFamily.Domain.PetTypeManagment.Root.Species;

namespace PetFamily.Application.Tests;

public class AddPetHandlerTests
{
    private readonly Mock<ILogger<AddPetHandler>> _loggerMock;
    private readonly Mock<IVolunteerWriteRepository> _volunteerRepositoryMock;
    private readonly Mock<ISpeciesWriteRepository> _speciesRepositoryMock;
    private readonly AddPetHandler _handler;
    private readonly CancellationToken _token;
    private readonly Mock<IVolunteerReadRepository> _volunteerReadRepositoryMock;
    private readonly Mock<ISpeciesReadRepository> _speciesReadRepository;

    public AddPetHandlerTests()
    {
        _loggerMock = new Mock<ILogger<AddPetHandler>>();
        _volunteerRepositoryMock = new Mock<IVolunteerWriteRepository>();
        _speciesRepositoryMock = new Mock<ISpeciesWriteRepository>();
        _volunteerReadRepositoryMock = new Mock<IVolunteerReadRepository>();
        _speciesReadRepository = new Mock<ISpeciesReadRepository>();

        _handler = new AddPetHandler(
            _loggerMock.Object,
            _volunteerRepositoryMock.Object,
            _speciesReadRepository.Object,
            _speciesRepositoryMock.Object);

        _token = CancellationToken.None;
    }

    [Theory]
    [InlineData("volunteerId", false, "Lesy", 50, 0, 0, "red", true, true, "+373", "69961151", 50, 0, "Trento", "jrfoe", "ehbhb", "21a")]//Volunteer GuidEmpty
    [InlineData("petNamePattern", true, "Lesy?!%", 50, 0, 0, "red", true, true, "+373", "69961151", 50, 0, "Trento", "jrfoe", "ehbhb", "21a")]//pet name doesnt mutch pattern 
    [InlineData("descriptionLength", true, "Lesy", 2000, 0, 0, "red", true, true, "+373", "69961151", 50, 0, "Trento", "jrfoe", "ehbhb", "21a")]//descriptionSize is too big!
    [InlineData("weight", true, "Lesy", 50, -1, 0, "red", true, true, "+373", "69961151", 50, 0, "Trento", "jrfoe", "ehbhb", "21a")]//Weight value is negative
    [InlineData("height", true, "Lesy", 50, 0, -1, "red", true, true, "+373", "69961151", 50, 0, "Trento", "jrfoe", "ehbhb", "21a")]//HeightValue is negative
    [InlineData("colorPattern", true, "Lesy", 50, 0, 0, "red!?", true, true, "+373", "69961151", 50, 0, "Trento", "jrfoe", "ehbhb", "21a")]//color doesnt mutch pattern
    [InlineData("speciesId", true, "Lesy", 50, 0, 0, "red", false, true, "+373", "69961151", 50, 0, "Trento", "jrfoe", "ehbhb", "21a")]//Species GuidEmpty
    [InlineData("breedId", true, "Lesy", 50, 0, 0, "red", true, false, "+373", "69961151", 50, 0, "Trento", "jrfoe", "ehbhb", "21a")]//Breed GuidEmpty
    [InlineData("phoneRegionCode", true, "Lesy", 50, 0, 0, "red", true, true, "", "69961151", 50, 0, "Trento", "jrfoe", "ehbhb", "21a")]//phoneRegion doesnt mutch pattern
    [InlineData("phoneNumber", true, "Lesy", 50, 0, 0, "red", true, true, "+373", "", 50, 0, "Trento", "jrfoe", "ehbhb", "21a")]//phoneNumber doesnt mutch pettern
    [InlineData("healthInfoLength", true, "Lesy", 50, 0, 0, "red", true, true, "+373", "69961151", 5000, 0, "Trento", "jrfoe", "ehbhb", "21a")]//Health info size is too big
    [InlineData("helpStatusNumber", true, "Lesy", 50, 0, 0, "red", true, true, "+373", "69961151", 50, 100, "Trento", "jrfoe", "ehbhb", "21a")]//Help status doesnt exist
    [InlineData("cityPattern", true, "Lesy", 50, 0, 0, "red", true, true, "+373", "69961151", 50, 0, "Trento%!331", "jrfoe", "ehbhb", "21a")]//City doesnt mutch its pattern
    public async Task Handle_ShouldReturnValidationError_WhenCommand_IsInvalid(
        string invalidField,
        bool volunteerId,
        string nickName,
        int descriptionSize,
        double weight,
        double height,
        string color,
        bool speciesId,
        bool breedId,
        string phoneRegionCode,
        string phoneNumber,
        int healthInfoSize,
        int helpStatusNumber,
        string city,
        string region,
        string street,
        string homeNumber)
    {
        // ARRANGE
        Guid VolunteerId = volunteerId ? Guid.NewGuid() : Guid.Empty;
        Guid SpeciesId = speciesId ? Guid.NewGuid() : Guid.Empty;
        Guid BreedId = breedId ? Guid.NewGuid() : Guid.Empty;

        var invalidCommand = new AddPetCommand(
            VolunteerId: VolunteerId,
            PetName: nickName,
            DateOfBirth: null,
            Description: new string('a', descriptionSize),
            IsVaccinated: true,
            IsSterilized: true,
            Weight: weight,
            Height: height,
            Color: color,
            SpeciesId: SpeciesId,
            BreedId: BreedId,
            OwnerPhoneRegion: phoneRegionCode,
            OwnerPhoneNumber: phoneNumber,
            HealthInfo: new string('A', healthInfoSize),
            HelpStatus: helpStatusNumber,
            City: city,
            Region: region,
            Street: street,
            HomeNumber: homeNumber,
            []);

        // ACT
        var result = await _handler.Handle(invalidCommand, _token);

        // ASSERT
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
        Assert.Single(result.Error.ValidationErrors);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundError_WhenBreedOrSpecies_DoesNotExist()
    {
        // ARRANGE
        DomainSpecies? nullSpecies = null;

        var command = new AddPetCommand(
            VolunteerId: Guid.NewGuid(),
            PetName: "NickName",
            DateOfBirth: null,
            Description: new string('a', 50),
            IsVaccinated: true,
            IsSterilized: true,
            Weight: 0,
            Height: 0,
            Color: "color",
            SpeciesId: Guid.NewGuid(),
            BreedId: Guid.NewGuid(),
            OwnerPhoneRegion: "+373",
            OwnerPhoneNumber: "69961151",
            HealthInfo: new string('A', 50),
            HelpStatus: 0,
            City: "City",
            Region: "region",
            Street: "AUgust",
            HomeNumber: "11a",
            []);

        _speciesReadRepository
            .Setup(x => x.CheckIfPetTypeExists(command.SpeciesId, command.BreedId, _token))
            .ReturnsAsync(UnitResult.Fail(Error.NotFound("Species")));

        // ACT
        var result = await _handler.Handle(command, _token);

        // ASSERT
        Assert.False(result.IsSuccess);
        Assert.Equal(Error.NotFound("Species").Type, result.Error.Type);
        Assert.Equal(Error.NotFound("Species").Message, result.Error.Message);

    }

    [Fact]
    public async Task Handle_ShouldReturnUnitResultOkWithAddPetResponse()
    {
        // ARRANGE
        DomainSpecies species = DomainSpecies.Create(SpeciesID.NewGuid(), "dog").Data!;
        var breed = Breed.Create(BreedID.NewGuid(), "pitbul", "").Data!;
        species.AddBreed(breed);
        Volunteer mockVolunteer = TestDataFactory.CreateVolunteer();
        var resultGetVolunteer = Result.Ok(mockVolunteer);

        var command = new AddPetCommand(
            VolunteerId: Guid.NewGuid(),
            PetName: "NickName",
            DateOfBirth: null,
            Description: new string('a', 50),
            IsVaccinated: true,
            IsSterilized: true,
            Weight: 0,
            Height: 0,
            Color: "color",
            SpeciesId: species.Id,
            BreedId: breed.Id,
            OwnerPhoneRegion: "+373",
            OwnerPhoneNumber: "69961151",
            HealthInfo: new string('A', 50),
            HelpStatus: 0,
            City: "City",
            Region: "region",
            Street: "AUgust",
            HomeNumber: "11a",
            []);

        _speciesReadRepository
            .Setup(x => x.CheckIfPetTypeExists(command.SpeciesId, command.BreedId, _token))
            .ReturnsAsync(UnitResult.Ok());

        _volunteerRepositoryMock
           .Setup(x => x.GetByIdAsync(command.VolunteerId, _token))
           .ReturnsAsync(resultGetVolunteer);

        _volunteerRepositoryMock
            .Setup(x => x.Save(resultGetVolunteer.Data!, _token))
            .ReturnsAsync(UnitResult.Ok());

        //ACT
        var result = await _handler.Handle(command, _token);

        // ASSERT
        Assert.True(result.IsSuccess);
        Assert.Single(mockVolunteer.Pets);
        Assert.Equal(1, mockVolunteer.Pets[0].SerialNumber.Value);
    }
}

