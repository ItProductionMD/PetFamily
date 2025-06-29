using FileStorage.Application.IRepository;
using FileStorage.Application.Validations;
using FileStorage.Public.Contracts;
using FileStorage.Public.Dtos;
using FileStorage.SharedFramework.IFormFiles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.Utilities;


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
    public async Task<ActionResult> UploadFile(
        IFormFile file,
        CancellationToken cancellationToken = default)
    {
        var validateFile = IFormFileValidator.ValidateFile(file, _fileValidationOptions);
        if (validateFile.IsFailure)
            return BadRequest(validateFile.Error);

        using var stream = file.OpenReadStream();

        UploadFileDto appFile = file.ToUploadFileDto(TEST_BUCKET_NAME, stream);

        await _fileRepository.UploadFileAsync(appFile, cancellationToken);

        return Ok(appFile.StoredName);
    }

    //-----------------------------------------UploadFiles-----------------------------------------//
    /// <summary>
    /// Upload files
    /// </summary>
    /// <param name="file"></param>
    /// <param name="cancelToken"></param>
    /// <returns></returns>
    [HttpPost("files")]
    public async Task<ActionResult> UploadFiles(
        [FromForm] List<IFormFile> files,
        CancellationToken cancelToken = default)
    {
        var validate = IFormFileValidator.ValidateFiles(files, _fileValidationOptions);
        if (validate.IsFailure)
            return BadRequest(validate.Error);

        await using var disposableStreams = new AsyncDisposableCollection();

        var fileDtos = files.ToUploadFileDtos(TEST_BUCKET_NAME, disposableStreams);

        var uploadResult = await _fileRepository.UploadFilesAsync(fileDtos, cancelToken);
        if (uploadResult.IsFailure)
            return BadRequest(uploadResult.Error);

        return Ok(uploadResult.Data);
    }

    //-----------------------------------------GetFileUrl-----------------------------------------//
    /// <summary>
    /// Get file url
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    [HttpGet("{fileName}/url")]
    public async Task<ActionResult> GetFileUrl(
        [FromRoute] string fileName,
        CancellationToken cancelToken)
    {
        var result = await _fileRepository.GetFileUrlAsync(new(fileName, TEST_BUCKET_NAME), cancelToken);
        return result.IsFailure
            ? NotFound(result.Error.Message)
            : Ok(result.Data!);
    }

    //---------------------------------------DeleteFile-------------------------------------------//
    /// <summary>
    /// Delete file
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    [HttpDelete("{fileName}")]
    public async Task<ActionResult<Result>> SoftDeleteFile(
        [FromRoute] string fileName,
        CancellationToken cancelToken)
    {
        DeleteFileDto command = new(fileName);

        await _fileRepository.SoftDeleteFileAsync(new(fileName, TEST_BUCKET_NAME), cancelToken);

        return Ok("File deleted successfully!");
    }

    /// <summary>
    /// DeleteFiles
    /// </summary>
    /// <param name="fileNames"></param>
    /// <param name="cancelToken"></param>
    /// <returns></returns>
    [HttpDelete("files")]
    public async Task<ActionResult> DeleteFiles(
        [FromBody] List<string> fileNames,
        CancellationToken cancelToken)
    {
        List<FileDto> files = fileNames
            .Select(f => new FileDto(f, TEST_BUCKET_NAME))
            .ToList();

        var result = await _fileRepository.DeleteFileListAsync(files, cancelToken);
        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Data);
    }

    [HttpDelete("files/withQueue")]
    public async Task<ActionResult> DeleteFilesWithQueue(
        [FromBody] List<string> fileNames,
        CancellationToken cancelToken)
    {
        List<FileDto> files = fileNames
            .Select(f => new FileDto(f, TEST_BUCKET_NAME))
            .ToList();

        await _fileService.DeleteFilesUsingMessageQueue(files, cancelToken);

        return Ok("File deleted successfully!");
    }

    //----------------------------------Restore files---------------------------------------------//
    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="cancelToken"></param>
    /// <returns></returns>
    [HttpPost("{fileName}/restore")]
    public async Task<ActionResult<Result>> RestoreFile(
        [FromRoute] string fileName,
        CancellationToken cancelToken)
    {
        DeleteFileDto command = new(fileName);

        await _fileRepository.RestoreFileAsync(new(fileName, TEST_BUCKET_NAME), cancelToken);

        return Ok("File Restored successfully!");
    }
}
