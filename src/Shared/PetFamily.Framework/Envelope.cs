using PetFamily.SharedKernel.Errors;

namespace PetFamily.Framework
{
    public record Envelope
    {
        public object? Data { get; }
        public DateTime Timestamp { get; }
        public Error? Errors { get; }

        private Envelope(object? data, Error? error)
        {
            Data = data;
            Timestamp = DateTime.UtcNow;
            Errors = error;
        }
        public static Envelope Success(object? data) => new(data, null);

        public static Envelope Failure(Error error, object? data = default) => new(data, error);
    }

}
