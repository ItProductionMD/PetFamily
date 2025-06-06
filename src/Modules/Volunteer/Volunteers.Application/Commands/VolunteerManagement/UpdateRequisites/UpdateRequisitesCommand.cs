﻿using PetFamily.Application.Abstractions.CQRS;
using Volunteers.Application.ResponseDtos;

namespace Volunteers.Application.Commands.VolunteerManagement.UpdateRequisites;

public record class UpdateRequisitesCommand(
    Guid VolunteerId,
    IEnumerable<RequisitesDto> RequisitesDtos) : ICommand;

