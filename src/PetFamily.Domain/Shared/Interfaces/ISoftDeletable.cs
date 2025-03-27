namespace PetFamily.Domain.Shared.Interfaces;

public interface ISoftDeletable
{
    void Delete();
    void Restore();
}
