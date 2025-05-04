namespace PetFamily.Domain.Shared.Interfaces;

public interface ISoftDeletable
{
    void SoftDelete();
    void Restore();
}
