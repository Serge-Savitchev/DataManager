using DataManagerAPI.Dto;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Models;
using DataManagerAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

namespace DataManagerAPI.Controllers;

/// <summary>
/// Files controller.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class UserFilesController : ControllerBase
{
    private readonly IUserFileService _service;
    private readonly int _defaultBufferSize = 1024 * 4;
    private readonly bool _useBufferingStreamCopy;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="service"></param>
    /// <param name="configuration"></param>
    public UserFilesController(IUserFileService service, IConfiguration configuration)
    {
        _service = service;
        if (!bool.TryParse(configuration["Buffering:Client:UseTemporaryFile"], out _useBufferingStreamCopy))
        {
            _useBufferingStreamCopy = false;
        }

        if (int.TryParse(configuration["Buffering:Client:BufferSize"], out int size) && size > 0)
        {
            _defaultBufferSize = size * 1024;
        }
    }

    /// <summary>
    /// Dowloads file from database.
    /// </summary>
    /// <param name="userDataId"></param>
    /// <param name="fileId"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(UserFileStreamDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet]
    [Route("Download")]
    public async Task<IActionResult> DownloadFile([FromQuery] int userDataId, [FromQuery] int fileId)
    {
        ResultWrapper<UserFileStreamDto> ret = await _service.DownloadFileAsync(userDataId, fileId);
        if (!ret.Success)
        {
            return NotFound();
        }

        new FileExtensionContentTypeProvider()
                        .TryGetContentType(ret.Data!.Name, out string? contentType);
        return File(ret.Data.Content!, contentType ?? "application/octet-stream", ret.Data.Name);
    }

    /// <summary>
    /// Returns list of all files in database for UserDataDto"/>.
    /// </summary>
    /// <param name="userDataId"></param>
    /// <returns>Array of files <see cref="UserFileDto"/>.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(UserFileDto[]), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetList([FromQuery] int userDataId)
    {
        ResultWrapper<UserFileDto[]> ret = await _service.GetListAsync(userDataId);

        if (!ret.Success || ret.Data == null)
        {
            return BadRequest();
        }

        return Ok(ret.Data);
    }

    /// <summary>
    /// Deletes file from database.
    /// </summary>
    /// <param name="userDataId"></param>
    /// <param name="fileId"></param>
    /// <returns>Id of deleted file.</returns>
    [HttpDelete]
    public async Task<IActionResult> DeleteFile([FromQuery] int userDataId, [FromQuery] int fileId)
    {
        ResultWrapper<int> ret = await _service.DeleteFileAsync(userDataId, fileId);

        return Ok(ret.Data);
    }


#pragma warning disable CS1572 // XML comment has a param tag, but there is no parameter by that name
    /// <summary>
    /// Uploads file to database. Multipart/form-data is used. If there are several form-data only the first one will be processed.
    /// </summary>
    /// <param name="userDataId"></param>
    /// <param name="fileId"></param>
    /// <param name="BigFile">Boolean flag ("true"/"false"). It "true" special feature of storing of big files in database is used.</param>
    /// <returns><see cref="UserFile"/></returns>
    [HttpPost]
#pragma warning restore CS1572 // XML comment has a param tag, but there is no parameter by that name
    [Route("Upload")]
    [DisableRequestSizeLimit]
    [ProducesResponseType(typeof(UserFileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadFile()
    {
        int.TryParse(Request.Query["UserDataId"], out int userDataId);
        int.TryParse(Request.Query["FileId"], out int fileId);
        bool.TryParse(Request.Query["BigFile"], out bool bigFile);

        string? boundary = HeaderUtilities.RemoveQuotes(
            MediaTypeHeaderValue.Parse(Request.ContentType).Boundary
        ).Value;

        var reader = new MultipartReader(boundary!, Request.Body, _defaultBufferSize);

        var file = new UserFile { Id = fileId, UserDataId = userDataId };
        UserFileDto? result = await UploadFile(reader, file, bigFile);

        if (result == null)
        {
            return BadRequest();
        }

        return Ok(result);
    }

    private async Task<UserFileDto?> UploadFile(MultipartReader reader, UserFile file, bool bigFile)
    {
        UserFileDto? uploadedFile = null;  // result

        var section = await reader.ReadNextSectionAsync();

        if (section != null)
        {
            bool hasHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition);

            if (hasHeader
                && contentDisposition != null
                && contentDisposition.DispositionType.Equals("form-data")
                && !string.IsNullOrEmpty(contentDisposition.FileName.Value))
            {
                var uploadData = new UserFileStreamDto
                {
                    Id = file!.Id,
                    UserDataId = file.UserDataId,
                    BigFile = bigFile,
                    Name = contentDisposition.FileName.Value!
                };

                string? tmpFile = null;

                try
                {
                    Stream? uploadStream = null;

                    if (_useBufferingStreamCopy)
                    {
                        tmpFile = await CopyStreamToFile(section.Body, uploadData.Name);
                        uploadStream = new System.IO.FileStream(tmpFile, FileMode.Open, FileAccess.Read);
                    }
                    else
                    {
                        uploadStream = section.Body;
                    }

                    await using BufferedStream bufferedStream = new(uploadStream, _defaultBufferSize);
                    uploadData.Content = bufferedStream;

                    ResultWrapper<UserFileDto> result = await _service.UploadFileAsync(uploadData);
                    if (result.Success && result?.Data != null)
                    {
                        uploadedFile = result.Data;
                    }
                }
                finally
                {
                    if (tmpFile != null)
                    {
                        Directory.Delete(Path.GetDirectoryName(tmpFile)!, true);
                    }
                }
            }
        }

        return uploadedFile;
    }

    private static async Task<string> CopyStreamToFile(Stream inputStream, string fileName)
    {
        DirectoryInfo tempDir = Directory.CreateTempSubdirectory("DataManagerAPI");
        string fullName = Path.Combine(tempDir.FullName, fileName);

        using var outStream = new System.IO.FileStream(fullName, FileMode.CreateNew);
        await inputStream.CopyToAsync(outStream);

        return fullName;
    }

}
