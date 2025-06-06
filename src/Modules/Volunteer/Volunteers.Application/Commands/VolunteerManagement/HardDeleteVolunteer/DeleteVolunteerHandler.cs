﻿using FileStorage.Public.Contracts;
using FileStorage.Public.Dtos;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions.CQRS;
using PetFamily.SharedKernel.Results;
using Volunteers.Application.IRepositories;

namespace Volunteers.Application.Commands.VolunteerManagement.DeleteVolunteer;

public class DeleteVolunteerHandler(
    ILogger<DeleteVolunteerHandler> logger,
    IVolunteerWriteRepository volunteerRepository,
    IFileService fileService) : ICommandHandler<Guid, HardDeleteVolunteerCommand>
{
    private readonly IVolunteerWriteRepository _repository = volunteerRepository;
    private readonly ILogger<DeleteVolunteerHandler> _logger = logger;
    IFileService _fileService = fileService;
    public async Task<Result<Guid>> Handle(HardDeleteVolunteerCommand cmd, CancellationToken ct)
    {
        var getVolunteer = await _repository.GetByIdAsync(cmd.VolunteerId, ct);
        if (getVolunteer.IsFailure)
            return Result.Fail(getVolunteer.Error);

        var volunteer = getVolunteer.Data!;

        List<FileDto> imagesToDelete = [];

        foreach (var pet in volunteer.Pets)
            imagesToDelete.AddRange(pet.Images.Select(x =>
                new FileDto(x.Name, Constants.BUCKET_FOR_PET_IMAGES)));

        await _repository.Delete(volunteer, ct);

        if (imagesToDelete.Count > 0)
            await _fileService.DeleteFilesUsingMessageQueue(imagesToDelete);

        _logger.LogInformation("Hard delete volunteer with id:{Id} successful!", cmd.VolunteerId);

        return Result.Ok(volunteer.Id);
    }
}
