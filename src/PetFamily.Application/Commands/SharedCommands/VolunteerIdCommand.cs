﻿
using PetFamily.Application.Abstractions;

namespace PetFamily.Application.Commands.SharedCommands;

public record VolunteerIdCommand(Guid VolunteerId):ICommand;

