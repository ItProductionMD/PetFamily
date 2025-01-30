using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PetFamily.API.Responce;
using PetFamily.Application;
using PetFamily.Domain.Shared.DomainResult;

namespace PetFamily.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestFileServiceController : ControllerBase
{
    private readonly IAppiFileService _fileService;
    public TestFileServiceController(IAppiFileService fileService)
    {
        _fileService = fileService;
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
        using var stream = file.OpenReadStream();
        var result = await _fileService.UploadFileAsync(
            "test",
            Guid.NewGuid().ToString(),
            stream, 
            cancellationToken);

        if (result.IsFailure)
            return BadRequest(Envelope.Failure(result.Errors));

        return Ok(Envelope.Success(result.Data));
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
        var result = await _fileService.GetFileUrl("test", objectName, default);
        if (result.IsFailure)
            return NotFound(Envelope.Failure(result.Errors));

        return Ok(Envelope.Success(result.Data));
    }

    //---------------------------------------DeleteFile-------------------------------------------//
    /// <summary>
    /// Delete file
    /// </summary>
    /// <param name="objectName"></param>
    /// <returns></returns>
    [HttpDelete("{objectName}")]
    public async Task<ActionResult<Result<Envelope>>> DeleteFile([FromRoute] string objectName)
    {
        var result = await _fileService.RemoveFileAsync("test", objectName, default);

        if (result.IsFailure)
            return NotFound(Envelope.Failure(result.Errors));

        return Ok(Envelope.Success("File deleted successfully!"));
    }
}
