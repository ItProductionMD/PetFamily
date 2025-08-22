using PetFamily.SharedKernel.Results;

namespace Account.Application.IServices;

public interface IPasswordHasher
{
    string Hash(string password);
    UnitResult Verify(string password, string hash);
}
