using PetFamily.SharedKernel.Results;

namespace PetFamily.Auth.Application.IServices;

public interface IPasswordHasher
{
    string Hash(string password);
    UnitResult Verify(string password, string hash);
}
