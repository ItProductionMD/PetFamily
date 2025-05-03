using Microsoft.Extensions.Logging;
using PetFamily.Domain.Results;
using Microsoft.Extensions.Options;
using PetFamily.Application.IRepositories;
using PetFamily.Application.Commands.FilesManagment;
using PetFamily.Application.Commands.FilesManagment.Dtos;
using PetFamily.Application.Commands.SharedCommands;
using PetFamily.Application.Abstractions;

namespace PetFamily.Application.Commands.VolunteerManagment.DeleteVolunteer;

public class SoftDeleteVolunteerHandler(
    ILogger<SoftDeleteVolunteerHandler> logger,
    IVolunteerWriteRepository volunteerRepository,
    FilesProcessingQueue filesProcessingQueue,
    IOptions<FileFolders> fileFoldersOptions) : ICommandHandler<Guid, SoftDeleteVolunteerCommand>
{
    private readonly IVolunteerWriteRepository _volunteerRepository = volunteerRepository;
    private readonly ILogger<SoftDeleteVolunteerHandler> _logger = logger;
    FilesProcessingQueue _filesProcessingQueue = filesProcessingQueue;
    FileFolders _fileFolders = fileFoldersOptions.Value;

    public async Task<Result<Guid>> Handle(SoftDeleteVolunteerCommand command, CancellationToken cancelToken)
    {
        var getVolunteer = await _volunteerRepository.GetByIdAsync(command.VolunteerId, cancelToken);
        if (getVolunteer.IsFailure)
            return Result.Fail(getVolunteer.Error);

        var volunteer = getVolunteer.Data!;

        List<AppFileDto> imagesToDelete = [];

        foreach (var pet in volunteer.Pets)
            imagesToDelete.AddRange(pet.Images.Select(i => new AppFileDto(i.Name, _fileFolders.Images)));
        
        volunteer.SoftDelete();

        var result = await _volunteerRepository.Save(volunteer, cancelToken);
        if (result.IsFailure)
            return result;

        if (imagesToDelete.Count > 0)
            await _filesProcessingQueue.DeleteChannel.Writer.WriteAsync(imagesToDelete);
        
       _logger.LogInformation("Softdelete volunteer with id:{volunteerId} successful!",command.VolunteerId);

        return Result.Ok(volunteer.Id);
    }
}
