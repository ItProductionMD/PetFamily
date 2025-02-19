using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.Extensions.Options;
using PetFamily.API.Dtos;
using PetFamily.API.Extensions;
using PetFamily.API.Responce;
using PetFamily.Application.FilesManagment;
using PetFamily.Application.FilesManagment.Commands;
using PetFamily.Application.FilesManagment.Dtos;
using PetFamily.Application.SharedValidations;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.Results;
using System.Runtime.CompilerServices;

namespace PetFamily.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestFileServiceController : ControllerBase
{
    public const string folderName = "test";
    private readonly IFileRepository _fileService;
    private readonly FileValidatorOptions _fileValidatorOptions;
    public TestFileServiceController(
        IFileRepository fileService,
        IOptions<FileValidatorOptions> fileValidatorOptions)
    {
        _fileService = fileService;
        _fileValidatorOptions = fileValidatorOptions.Value;
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

        UploadFileCommand fileDto = new(
               string.Concat(Guid.NewGuid(), fileExtension),
               file.ContentType.ToLower(),
               file.Length,
               fileExtension,
               file.OpenReadStream());
        //------------------------------Validate fileDto------------------------------------------//
        var validateFile = FileValidator.Validate(fileDto, _fileValidatorOptions);
        if (validateFile.IsFailure)
            return BadRequest(validateFile.ToErrorActionResult());
        try
        {
            await _fileService.UploadFileAsync(folderName, fileDto, cancellationToken);
            return Ok(Envelope.Success(fileDto.StoredName));
        }
        finally
        {
            fileDto.Stream?.Dispose();
        }

    }
    //-----------------------------------------UploadFiles-----------------------------------------//
    /// <summary>
    /// Upload files
    /// </summary>
    /// <param name="file"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("(files)")]
    public async Task<ActionResult<Envelope>> UploadFiles(
        [FromForm] IFormFileCollection files,
        CancellationToken cancellationToken = default)
    {
        if (files.Count == 0)
            return Result.Ok("No files for uploading found").ToEnvelope();

        //--------------------------------Validate max files count--------------------------------//
        var validateCount = FileValidator.ValidateMaxCount(files.Count, _fileValidatorOptions);
        if (validateCount.IsFailure)
            return validateCount.ToErrorActionResult();
        //-----------------Get Validation Errors,commandsForUpload and listForResponse------------//
        CreatingDataForUpload(
            files,
            out var commandsForUpload,
            out var responseList,
            out var validationErrors);

        try
        {
            if (validationErrors.Count == files.Count)
                return Result.Fail(validationErrors).ToErrorActionResult();

            var uploadResult = await _fileService.UploadFileListAsync(
                folderName,
                commandsForUpload,
                cancellationToken);

            if (validationErrors.Count > 0)
                uploadResult.AddErrors(validationErrors!);

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
            foreach (var fileDto in commandsForUpload)
                fileDto.Stream?.Dispose();
        }
    }
    //-----------------------------------------GetFileUrl-----------------------------------------//
    /// <summary>
    /// Get file url
    /// </summary>
    /// <param name="objectName"></param>
    /// <returns></returns>
    [HttpGet("{objectName}/url")]
    public async Task<ActionResult<Envelope>> GetFileUrl([FromRoute] string objectName)
    {
        var result = await _fileService.GetFileUrlAsync(folderName, objectName, default);
        if (result.IsFailure)
            return NotFound(Envelope.Failure(result.Errors));

        return Ok(Envelope.Success(result.Data!));
    }

    //---------------------------------------DeleteFile-------------------------------------------//
    /// <summary>
    /// Delete file
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    [HttpDelete("{fileName}")]
    public async Task<ActionResult<Result<Envelope>>> DeleteFile(
        [FromRoute] string fileName,
        CancellationToken cancellationToken)
    {
        DeleteFileCommand command = new(fileName);

        await _fileService.DeleteFileAsync(folderName, command, cancellationToken);

        return Ok(Envelope.Success("File deleted successfully!"));
    }

    [HttpDelete("{fileNames}/files")]
    public async Task<ActionResult<Envelope>> DeleteFiles(
        [FromBody] List<DeleteFileCommand> commands,
        CancellationToken cancellationToken)
    {
        var result = await _fileService.DeleteFileListAsync(folderName, commands, cancellationToken);
        if (result.IsFailure)
        {
            if (result.Data == null || result.Data.Count == 0)
                return result.ToErrorActionResult();

        }
        List<FileUploadResponse> responseList = [];
        foreach (var file in commands)
        {
            if (result.Data!.FirstOrDefault(f => f == file.StoredName) == null)
                responseList.Add(new(file.StoredName, file.StoredName) { IsUploaded = false });          
            else           
                responseList.Add(new(file.StoredName, file.StoredName) { IsUploaded = true });          
        }
        return Result.Ok(responseList).ToEnvelope();
    }

    //-----------------------------------Private methods------------------------------------------//
    private void CreatingDataForUpload(
        IFormFileCollection files,
        out List<UploadFileCommand> commandsForUpload,
        out List<FileUploadResponse> responseList,
        out List<Error> validationErrors)
    {
        validationErrors = [];
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
            var validateFileDto = FileValidator.Validate(fileDto, _fileValidatorOptions);
            if (validateFileDto.IsFailure)
            {
                stream.Dispose();

                validationErrors.AddRange(validateFileDto.Errors);

                fileForResponse.Error = string.Join(
                    "|",
                    validateFileDto.Errors.Select(e => e.Message).ToList());
            }
            else
            {
                commandsForUpload.Add(fileDto);
                fileForResponse.Error = "Uploadind file server error";
            }

            responseList.Add(fileForResponse);
        }
    }
}
