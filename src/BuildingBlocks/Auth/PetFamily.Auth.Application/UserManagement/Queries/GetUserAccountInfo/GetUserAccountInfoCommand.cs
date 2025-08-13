using PetFamily.SharedApplication.Abstractions.CQRS;

namespace PetFamily.Auth.Application.UserManagement.Queries.GetUserAccountInfo;

public record GetUserAccountInfoCommand(Guid userId) : ICommand;

