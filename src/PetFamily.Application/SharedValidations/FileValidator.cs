using PetFamily.Application.Commands.FilesManagment.Commands;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.Results;
using PetFamily.Domain.Shared.Validations;

namespace PetFamily.Application.SharedValidations;

public static class FileValidator
{
    public static UnitResult Validate(UploadFileCommand fileDto, FileValidatorOptions options)
    {
        List<ValidationError> validationErrors = [];

        if (fileDto.Stream == null || fileDto.Size == 0)
            validationErrors.Add(
                Error.FileValidation(
                    fileDto.OriginalName,
                    ValidationErrorCodes.FILE_IS_EMPTY)
                .ValidationErrors.FirstOrDefault()!);

        if (fileDto.Size > options.MaxSize)
            validationErrors.Add(
                Error.FileValidation(
                    fileDto.OriginalName,
                    ValidationErrorCodes.FILE_TOO_LARGE)
                .ValidationErrors.FirstOrDefault()!);

        if (!options.AllowedExtensions.Contains(fileDto.Extension))
            validationErrors.Add(
                 Error.FileValidation(
                     fileDto.OriginalName,
                     ValidationErrorCodes.FILE_INVALID_EXTENSION)
                 .ValidationErrors.FirstOrDefault()!);


        if (!options.AllowedMimeTypes.Contains(fileDto.MimeType))
            validationErrors.Add(
                 Error.FileValidation(
                     fileDto.OriginalName,
                     ValidationErrorCodes.FILE_INVALID_EXTENSION)
                 .ValidationErrors.FirstOrDefault()!);

        if (validationErrors.Count > 0)
            return UnitResult.Fail(Error.ValidationError(validationErrors));

        return UnitResult.Ok();
    }

    public static UnitResult ValidateFilesCount(int filesCount, FileValidatorOptions options)
    {
        if (filesCount == 0)
            return UnitResult.Fail(Error.FilesCountIsNull());

        if (filesCount > options.MaxFilesCount)
            return UnitResult.Fail(Error.ValueOutOfRange("Count of files is out of range"));

        return UnitResult.Ok();
    }

    public static UnitResult ValidateList(
        List<UploadFileCommand> fileDtos,
        FileValidatorOptions fileValidatorOptions)
    {
        var validateCounts = ValidateFilesCount(fileDtos.Count, fileValidatorOptions);
        if (validateCounts.IsFailure)
            return validateCounts;

        List<ValidationError> validationErrors = [];

        foreach (var fileDto in fileDtos)
        {
            var validationResult = Validate(fileDto, fileValidatorOptions);
            if (validationResult.IsFailure)
                validationErrors.AddRange(validationResult.Error.ValidationErrors);
        }
        if (validationErrors.Count > 0)
            return UnitResult.Fail(Error.ValidationError(validationErrors));

        return UnitResult.Ok();
    }
}
