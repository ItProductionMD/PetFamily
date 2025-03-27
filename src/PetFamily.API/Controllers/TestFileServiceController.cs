using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.Extensions.Options;
using PetFamily.API.Common.AppiValidators;
using PetFamily.API.Common.Utilities;
using PetFamily.API.Dtos;
using PetFamily.API.Extensions;
using PetFamily.API.Responce;
using PetFamily.Application.Commands.FilesManagment;
using PetFamily.Application.Commands.FilesManagment.Commands;
using PetFamily.Application.Commands.FilesManagment.Dtos;
using PetFamily.Application.IRepositories;
using PetFamily.Application.SharedValidations;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.Results;
using PetFamily.Domain.Shared.Validations;
using System.Runtime.CompilerServices;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace PetFamily.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestFileServiceController : ControllerBase
{
    private readonly FileFolders _fileFolders;
    private readonly IFileRepository _fileService;
    private readonly FileValidatorOptions _fileValidatorOptions;

    public TestFileServiceController(
        IFileRepository fileService,
        IOptions<FileFolders> fileFolders,
        IOptions<FileValidatorOptions> fileValidatorOptions)
    {
        _fileService = fileService;
        _fileValidatorOptions = fileValidatorOptions.Value;
        _fileFolders = fileFolders.Value;
    }

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
        var validateFile = Validators.ValidateFile(file, _fileValidatorOptions);
        if (validateFile.IsFailure)
            return validateFile.ToErrorActionResult();

        using var stream = file.OpenReadStream();

        AppFileDto appFile = Mappers.IFormFileToAppFileDto(file, _fileFolders.Images, stream);

        await _fileService.UploadFileAsync(appFile, cancellationToken);

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
        var validate = Validators.ValidateFiles(files, _fileValidatorOptions);
        if (validate.IsFailure)
            return validate.ToErrorActionResult();

        await using var disposableStreams = new AsyncDisposableCollection();

        var fileDtos = Mappers.IFormFilesToAppFileDtos(files, _fileFolders.Images, disposableStreams);

        var uploadResult = await _fileService.UploadFileListAsync(fileDtos, cancelToken);
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
        var result = await _fileService.GetFileUrlAsync(new(fileName, _fileFolders.Images), cancelToken);
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
        DeleteFileCommand command = new(fileName);

        await _fileService.SoftDeleteFileAsync(new(fileName, _fileFolders.Images), cancelToken);

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
        List<AppFileDto> files = fileNames
            .Select(f => new AppFileDto(f, _fileFolders.Images))
            .ToList();

        var result = await _fileService.DeleteFileListAsync(files, cancelToken);
        if (result.IsFailure)
            return result.ToErrorActionResult();

        return Result.Ok(result.Data).ToEnvelope();
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
        DeleteFileCommand command = new(fileName);

        await _fileService.RestoreFileAsync(new(fileName, _fileFolders.Images), cancelToken);

        return Ok(Envelope.Success("File Restored successfully!"));
    }
}
