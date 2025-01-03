
namespace PetFamily.Domain.Shared.ValueObjects
{
    public record GuidId
    {
        public Guid Id { get;}
        private GuidId(Guid id)
        {
            Id = id;
        }
        public static GuidId NewGuidId() => new (new Guid());
        public static GuidId Empty() => new (Guid.Empty);
    }
}
