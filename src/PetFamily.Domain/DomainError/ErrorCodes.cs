namespace PetFamily.Domain.DomainError;

public static class ErrorCodes
{
    public const string VALIDATION_ERROR = "validation.error";

    public const string DATA_NOT_FOUND = "data.not.found";

    public const string INTERNAL_SERVER_ERROR = "internal.server.error";

    public const string OPERATION_CANCELLED = "operation.cancelled";
    // authorization errors
    public const string ACCESS_DENIED = "access.denied"; 
    public const string ACCESS_FORBIDDEN = "access.forbidden";

    // database errors
    public const string DATABASE_ERROR = "database.error"; 
    public const string CONFLICT_ERROR = "database.conflict"; 
    public const string CONNECTION_ERROR = "database.connection.error"; 
}

