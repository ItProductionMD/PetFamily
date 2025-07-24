using FileStorage.Public.Dtos;
using PetFamily.SharedKernel.Results;

namespace FileStorage.Public.Contracts;

public interface IUploadFileDtoValidator
{
    UnitResult ValidateFiles(List<UploadFileDto> fileDtos, IFileValidatorOptions fileValidatorOptions);
}
