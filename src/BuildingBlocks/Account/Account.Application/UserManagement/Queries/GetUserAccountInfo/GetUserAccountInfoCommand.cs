using PetFamily.SharedApplication.Abstractions.CQRS;

namespace Account.Application.UserManagement.Queries.GetUserAccountInfo;

public record GetUserAccountInfoCommand(Guid userId) : ICommand;

