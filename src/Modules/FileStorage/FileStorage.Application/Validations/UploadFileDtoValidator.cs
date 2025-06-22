using FileStorage.Public.Contracts;
using FileStorage.Public.Dtos;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.Validations;

namespace FileStorage.Application.Validations;

public class UploadFileDtoValidator : IUploadFileDtoValidator
{
    public static UnitResult Validate(UploadFileDto fileDto, IFileValidatorOptions options)
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
            return UnitResult.Fail(Error.FromValidationErrors(validationErrors));

        return UnitResult.Ok();
    }

    public static UnitResult ValidateFilesCount(int filesCount, IFileValidatorOptions options)
    {
        if (filesCount == 0)
            return UnitResult.Fail(Error.FilesCountIsNull());

        if (filesCount > options.MaxFilesCount)
            return UnitResult.Fail(Error.ValueOutOfRange("Count of files is out of range"));

        return UnitResult.Ok();
    }

    public  UnitResult ValidateFiles(
        List<UploadFileDto> fileDtos,
        IFileValidatorOptions fileValidatorOptions)
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
            return UnitResult.Fail(Error.FromValidationErrors(validationErrors));

        return UnitResult.Ok();
    }
}
