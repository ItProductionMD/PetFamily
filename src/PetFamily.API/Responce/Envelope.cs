using PetFamily.Domain.DomainError;

namespace PetFamily.API.Responce
{
    public record Envelope
    {
        public Object? Data { get; }
        public DateTime Timestamp { get; }
        public Error? Errors { get; }

        private Envelope(object? data, Error? error)
        {
            Data = data;
            Timestamp = DateTime.UtcNow;
            Errors = error;
        }
        public static Envelope Success(Object? data) => new(data, null);

        public static Envelope Failure(Error error, Object? data = default) => new(data, error);
    }

}
