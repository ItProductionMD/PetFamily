using FileStorage.Application.Validations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetFamily.Framework;
using PetFamily.Framework.Extensions;
using PetFamily.Framework.FormFiles;
using PetFamily.Framework.Utilities;
using PetFamily.SharedKernel.Results;
using FileStorage.Application.IRepository;
using FileStorage.Public.Dtos;
using FileStorage.Public.Contracts;
using Microsoft.Extensions.Options;


namespace FileStorage.Presentation.Controllers;

[Route("api/files")]
[ApiController]
public class FileStorageController(
     IFileRepository fileRepository,
     IFileService fileService,
     IOptions<FileValidationTestOptions> fileOptions) : ControllerBase
{
    private readonly IFileService _fileService = fileService;
    private readonly IFileRepository _fileRepository = fileRepository;
    private readonly FileValidationTestOptions _fileValidationOptions = fileOptions.Value;
    private const string TEST_BUCKET_NAME = "testfilesversioned";

    //-----------------------------------------UploadFile-----------------------------------------//
    /// <summary>
    /// Upload file
    /// </summary>
    /// <param name="file"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<Envelope>> UploadFile(
        IFormFile file,
        CancellationToken cancellationToken = default)
    {
        var validateFile = FormFileValidator.ValidateFile(file, _fileValidationOptions);
        if (validateFile.IsFailure)
            return validateFile.ToErrorActionResult();

        using var stream = file.OpenReadStream();

        FileDto appFile = FormFileMapper.ToFileDto(file, TEST_BUCKET_NAME, stream);

        await _fileRepository.UploadFileAsync(appFile, cancellationToken);

        return Ok(Envelope.Success(appFile.Name));
    }

    //-----------------------------------------UploadFiles-----------------------------------------//
    /// <summary>
    /// Upload files
    /// </summary>
    /// <param name="file"></param>
    /// <param name="cancelToken"></param>
    /// <returns></returns>
    [HttpPost("files")]
    public async Task<ActionResult<Envelope>> UploadFiles(
        [FromForm] List<IFormFile> files,
        CancellationToken cancelToken = default)
    {
        var validate = FormFileValidator.ValidateFiles(files, _fileValidationOptions);
        if (validate.IsFailure)
            return validate.ToErrorActionResult();

        await using var disposableStreams = new AsyncDisposableCollection();

        var fileDtos = FormFileMapper.ToFileDtos(files, TEST_BUCKET_NAME, disposableStreams);

        var uploadResult = await _fileRepository.UploadFilesAsync(fileDtos, cancelToken);
        if (uploadResult.IsFailure)
            return uploadResult.ToErrorActionResult();

        return Ok(Envelope.Success(uploadResult.Data));
    }

    //-----------------------------------------GetFileUrl-----------------------------------------//
    /// <summary>
    /// Get file url
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    [HttpGet("{fileName}/url")]
    public async Task<ActionResult<Envelope>> GetFileUrl(
        [FromRoute] string fileName,
        CancellationToken cancelToken)
    {
        var result = await _fileRepository.GetFileUrlAsync(new(fileName, TEST_BUCKET_NAME), cancelToken);
        return result.IsFailure
            ? NotFound(Envelope.Failure(result.Error))
            : Ok(Envelope.Success(result.Data!));
    }

    //---------------------------------------DeleteFile-------------------------------------------//
    /// <summary>
    /// Delete file
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    [HttpDelete("{fileName}")]
    public async Task<ActionResult<Result<Envelope>>> SoftDeleteFile(
        [FromRoute] string fileName,
        CancellationToken cancelToken)
    {
        DeleteFileDto command = new(fileName);

        await _fileRepository.SoftDeleteFileAsync(new(fileName, TEST_BUCKET_NAME), cancelToken);

        return Ok(Envelope.Success("File deleted successfully!"));
    }

    /// <summary>
    /// DeleteFiles
    /// </summary>
    /// <param name="fileNames"></param>
    /// <param name="cancelToken"></param>
    /// <returns></returns>
    [HttpDelete("files")]
    public async Task<ActionResult<Envelope>> DeleteFiles(
        [FromBody] List<string> fileNames,
        CancellationToken cancelToken)
    {
        List<FileDto> files = fileNames
            .Select(f => new FileDto(f, TEST_BUCKET_NAME))
            .ToList();

        var result = await _fileRepository.DeleteFileListAsync(files, cancelToken);
        if (result.IsFailure)
            return result.ToErrorActionResult();

        return Result.Ok(result.Data).ToEnvelope();
    }

    [HttpDelete("files/withQueue")]
    public async Task<ActionResult<Envelope>> DeleteFilesWithQueue(
        [FromBody] List<string> fileNames,
        CancellationToken cancelToken)
    {
        List<FileDto> files = fileNames
            .Select(f => new FileDto(f, TEST_BUCKET_NAME))
            .ToList();

        await _fileService.DeleteFilesUsingMessageQueue(files, cancelToken);

        return Ok(Envelope.Success("File deleted successfully!"));
    }

    //----------------------------------Restore files---------------------------------------------//
    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="cancelToken"></param>
    /// <returns></returns>
    [HttpPost("{fileName}/restore")]
    public async Task<ActionResult<Result<Envelope>>> RestoreFile(
        [FromRoute] string fileName,
        CancellationToken cancelToken)
    {
        DeleteFileDto command = new(fileName);

        await _fileRepository.RestoreFileAsync(new(fileName, TEST_BUCKET_NAME), cancelToken);

        return Ok(Envelope.Success("File Restored successfully!"));
    }
}
