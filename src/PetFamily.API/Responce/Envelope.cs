using PetFamily.Domain.DomainError;

namespace PetFamily.API.Responce
{
    public record Envelope
    {
        public Object? Data { get;}
        public DateTime Timestamp { get;}

        public List<Error> Errors { get; } = [];

        private Envelope(object? data,List<Error>? errors)
        {
            Data = data;
            Timestamp = DateTime.UtcNow;
            Errors = errors ?? [];
        }

        public static Envelope Success(Object? data) => new(data,null);

        public static Envelope Failure(List<Error>? errors,Object? data = null) => new(data, errors);

    }

}
