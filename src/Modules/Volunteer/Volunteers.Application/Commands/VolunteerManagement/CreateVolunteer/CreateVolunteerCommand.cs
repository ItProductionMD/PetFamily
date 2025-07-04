﻿using PetFamily.Application.Abstractions.CQRS;
using Volunteers.Application.ResponseDtos;

namespace Volunteers.Application.Commands.VolunteerManagement.CreateVolunteer;

public record CreateVolunteerCommand(
    string FirstName,
    string LastName,
    string Description,
    int ExperienceYears,
    IEnumerable<RequisitesDto> Requisites) : ICommand;