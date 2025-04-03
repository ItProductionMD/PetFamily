namespace PetFamily.Domain.Shared.Interfaces;

public interface ISoftDeletable
{
    void SetAsDeleted();
    void Restore();
}
