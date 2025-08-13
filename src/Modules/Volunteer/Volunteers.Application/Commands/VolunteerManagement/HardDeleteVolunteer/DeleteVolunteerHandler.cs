using FileStorage.Public.Contracts;
using FileStorage.Public.Dtos;
using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Results;
using Volunteers.Application.IRepositories;

namespace Volunteers.Application.Commands.VolunteerManagement.DeleteVolunteer;

public class DeleteVolunteerHandler(
    IVolunteerWriteRepository volunteerWriteRepo,
    IFileService fileService,
    ILogger<DeleteVolunteerHandler> logger) : ICommandHandler<Guid, HardDeleteVolunteerCommand>
{
    public async Task<Result<Guid>> Handle(HardDeleteVolunteerCommand cmd, CancellationToken ct)
    {
        var getVolunteer = await volunteerWriteRepo.GetByIdAsync(cmd.VolunteerId, ct);
        if (getVolunteer.IsFailure)
            return Result.Fail(getVolunteer.Error);

        var volunteer = getVolunteer.Data!;

        List<FileDto> imagesToDelete = [];

        foreach (var pet in volunteer.Pets)
            imagesToDelete.AddRange(pet.Images.Select(x =>
                new FileDto(x.Name, Constants.BUCKET_FOR_PET_IMAGES)));

        await volunteerWriteRepo.Delete(volunteer, ct);

        if (imagesToDelete.Count > 0)
            await fileService.DeleteFilesUsingMessageQueue(imagesToDelete);

        logger.LogInformation("Hard delete volunteer with id:{Id} successful!", cmd.VolunteerId);

        return Result.Ok(volunteer.Id);
    }
}
