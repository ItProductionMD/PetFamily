namespace PetFamily.SharedKernel.Abstractions;
public interface ISoftDeletable
{
    void SoftDelete();
    void Restore();
}
