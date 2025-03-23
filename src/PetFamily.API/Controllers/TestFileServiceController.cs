using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.Extensions.Options;
using PetFamily.API.Dtos;
using PetFamily.API.Extensions;
using PetFamily.API.Responce;
using PetFamily.Application.Commands.FilesManagment;
using PetFamily.Application.Commands.FilesManagment.Commands;
using PetFamily.Application.Commands.FilesManagment.Dtos;
using PetFamily.Application.SharedValidations;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.Results;
using PetFamily.Domain.Shared.Validations;
using System.Runtime.CompilerServices;

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
        var fileExtension = UploadFileCommand.GetFullExtension(file.FileName);

        using var stream = file.OpenReadStream();
        if (stream == null)
            return Result.Fail(Error.InvalidFormat("File stream is null"))
                .ToErrorActionResult();

        UploadFileCommand command = new(
               string.Concat(Guid.NewGuid(), fileExtension),
               file.ContentType.ToLower(),
               file.Length,
               fileExtension,
               stream);
        //------------------------------Validate fileDto------------------------------------------//
        var validateFile = FileValidator.Validate(command, _fileValidatorOptions);
        if (validateFile.IsFailure)
            return BadRequest(validateFile.ToErrorActionResult());
        AppFile appFile = new(
            command.StoredName,
            _fileFolders.Images,
            command.Stream,
            command.Extension,
            command.MimeType,
            command.Size);

        await _fileService.UploadFileAsync(appFile, cancellationToken);

        return Ok(Envelope.Success(command.StoredName));
    }

    //-----------------------------------------UploadFiles-----------------------------------------//
    /// <summary>
    /// Upload files
    /// </summary>
    /// <param name="file"></param>
    /// <param name="cancelToken"></param>
    /// <returns></returns>
    [HttpPost("(files)")]
    public async Task<ActionResult<Envelope>> UploadFiles(
        [FromForm] IFormFileCollection files,
        CancellationToken cancelToken = default)
    {
        if (files.Count == 0)
            return Result.Ok("No files for uploading found").ToEnvelope();

        //--------------------------------Validate max files count--------------------------------//
        var validateCount = FileValidator.ValidateMaxCount(files.Count, _fileValidatorOptions);
        if (validateCount.IsFailure)
            return validateCount.ToErrorActionResult();
        //-----------------Get Validation Errors,commandsForUpload and listForResponse------------//
        var filesForUpload = CreatingDataForUpload(
            files, 
            out var uploadCommands, 
            out var responseList, 
            out var errorsAfterValidation);

        foreach(var error in errorsAfterValidation)
        {
            
        }
        if (errorsAfterValidation.Count == files.Count)
        {
            var result = UnitResult.Fail(errorsAfterValidation);
            return result.ToEnvelope();
        }
        try
        {
            var uploadResult = await _fileService.UploadFileListAsync(filesForUpload, cancelToken);

            if (errorsAfterValidation.Count > 0)
                uploadResult.AddValidationErrors(errorsAfterValidation!);

            var uploadedFileNames = uploadResult.Data ?? [];

            if (uploadedFileNames.Count == 0)
                return uploadResult.ToErrorActionResult();

            foreach (var name in uploadedFileNames)
            {
                var fileResponse = responseList.FirstOrDefault(f => f.StoredName == name)!;
                fileResponse.Error = string.Empty;
                fileResponse.IsUploaded = true;
            }
            return Ok(Envelope.Success(responseList));
        }
        finally
        {
            foreach (var file in uploadCommands)
                file.Stream?.Dispose();
        }
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

    [HttpDelete("{fileNames}/files")]
    public async Task<ActionResult<Envelope>> DeleteFiles(
        [FromBody] List<string> fileNames,
        CancellationToken cancelToken)
    {
        List<AppFile> files = fileNames.Select(f => new AppFile(f, _fileFolders.Images)).ToList();
        var result = await _fileService.DeleteFileListAsync(files, cancelToken);
        if (result.IsFailure)
        {
            if (result.Data == null || result.Data.Count == 0)
                return result.ToErrorActionResult();

        }
        List<FileUploadResponse> responseList = [];
        foreach (var name in fileNames)
        {
            if (result.Data!.FirstOrDefault(f => f == name) == null)
                responseList.Add(new(string.Empty, name) { IsUploaded = false });
            else
                responseList.Add(new(string.Empty, name) { IsUploaded = true });
        }
        return Result.Ok(responseList).ToEnvelope();
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

    //-----------------------------------Private methods------------------------------------------//
    private List<AppFile> CreatingDataForUpload(
        IFormFileCollection files,
        out List<UploadFileCommand> commandsForUpload,
        out List<FileUploadResponse> responseList,
        out List<Error> errors)
    {
        errors = [];
        commandsForUpload = [];
        responseList = [];
        foreach (var file in files)
        {
            var fileExtension = UploadFileCommand.GetFullExtension(file.FileName);
            var stream = file.OpenReadStream();

            UploadFileCommand fileDto = new(
                string.Concat(Guid.NewGuid(), fileExtension),
                file.ContentType.ToLower(),
                file.Length,
                fileExtension,
                stream);

            var fileForResponse = new FileUploadResponse(file.Name, fileDto.StoredName);
            //-----------------------------Validate fileDto---------------------------//
            var validationFileResult = FileValidator.Validate(fileDto, _fileValidatorOptions);
            if (validationFileResult.IsFailure)
            {
                stream.Dispose();

                errors.AddRange(validationFileResult.Error);

                fileForResponse.Error = string.Join(
                    "|",
                    validationFileResult.Error.ValidationErrors.Select(e => e.ToErrorMessage()).ToList());
            }
            else
            {
                commandsForUpload.Add(fileDto);
                fileForResponse.Error = "Uploadind file server error";
            }

            responseList.Add(fileForResponse);
        }
        return commandsForUpload.Select(c =>
               new AppFile(
                   c.StoredName,
                   _fileFolders.Images,
                   c.Stream,
                   c.Extension,
                   c.MimeType,
                   c.Size)).ToList();
    }
}
