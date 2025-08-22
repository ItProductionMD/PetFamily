using FileStorage.Public.Contracts;
using FileStorage.Public.Dtos;
using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Results;
using Volunteers.Application.IRepositories;

namespace Volunteers.Application.Commands.VolunteerManagement.SoftDeleteVolunteer;

public class SoftDeleteVolunteerHandler(
    IVolunteerWriteRepository VolunteerWriteRepo,
    IFileService fileService,
    ILogger<SoftDeleteVolunteerHandler> logger) : ICommandHandler<Guid, SoftDeleteVolunteerCommand>
{
    public async Task<Result<Guid>> Handle(SoftDeleteVolunteerCommand cmd, CancellationToken ct)
    {
        var getVolunteer = await VolunteerWriteRepo.GetByIdAsync(cmd.VolunteerId, ct);
        if (getVolunteer.IsFailure)
            return Result.Fail(getVolunteer.Error);

        var volunteer = getVolunteer.Data!;

        List<FileDto> imagesToDelete = [];

        foreach (var pet in volunteer.Pets)
            imagesToDelete.AddRange(pet.Images.Select(i =>
                new FileDto(i.Name, Constants.BUCKET_FOR_PET_IMAGES)));

        volunteer.SoftDelete();

        var result = await VolunteerWriteRepo.SaveAsync(volunteer, ct);
        if (result.IsFailure)
            return result;

        if (imagesToDelete.Count > 0)
            await fileService.DeleteFilesUsingMessageQueue(imagesToDelete);

        logger.LogInformation("Soft delete volunteer with id:{volunteerId} successful !", cmd.VolunteerId);

        return Result.Ok(volunteer.Id);
    }
}
