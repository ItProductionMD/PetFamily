namespace PetFamily.SharedKernel.Validations;

public static class ValidationErrorCodes
{
    //common validation error codes
    public const string VALUE_ALREADY_EXISTS = "value.already.exists";
    public const string VALUE_IS_EMPTY = "value.is.empty";
    public const string VALUE_INVALID_FORMAT = "value.invalid.format";
    public const string VALUE_INVALID_LENGTH = "value.invalid.length";
    public const string VALUE_OUT_OF_RANGE = "value.out.of.range";
    // file validation error codes
    public const string FILE_IS_EMPTY = "file.is.empty";
    public const string FILE_TOO_LARGE = "file.too.large";
    public const string FILE_INVALID_EXTENSION = "file.invalid.extension";
    public const string FILE_UPLOAD_FAILED = "file.upload.failed";
    public const string FILE_DELETE_FAILED = "file.delete.failed";
    public const string FILE_RESTORE_FAILED = "file.restore.failed";
    public const string FILES_COUNT_OUT_OF_RANGE = "files.count.out.of.range";
    public const string FILE_INVALID_MIME_TYPE = "file.invalid.mime.type";
    public const string FILES_COUNT_IS_NULL = "files.count.is.null";


}
