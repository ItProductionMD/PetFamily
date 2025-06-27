namespace PetFamily.SharedKernel.Abstractions;
public abstract class SoftDeletable
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public virtual void SoftDelete()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }
    public virtual void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
    }
}
