using DataManagerAPI.Dto;
using DataManagerAPI.Helpers;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Models;
using DataManagerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System.Diagnostics;

namespace DataManagerAPI.Controllers;

/// <summary>
/// Files controller.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UserFilesController : ControllerBase
{
    private readonly IUserFilesService _service;
    private readonly int _defaultBufferSize = 1024 * 4;
    private readonly bool _useTemporaryFile;

    // used for auto detection of "big file" mode.
    // if content length exceed this value the file is considered as "big" one.
    private readonly int _defaultBigFileSize = 50; // MB 

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="service"></param>
    /// <param name="configuration"></param>
    public UserFilesController(IUserFilesService service, IConfiguration configuration)
    {
        _service = service;

        if (!bool.TryParse(configuration["Buffering:Client:UseTemporaryFile"], out _useTemporaryFile))
        {
            _useTemporaryFile = false;
        }

        if (int.TryParse(configuration["Buffering:Client:BufferSize"], out int size) && size > 0)
        {
            _defaultBufferSize = size * 1024;
        }

        if (int.TryParse(configuration["Buffering:Client:BigFileSize"], out int bigFileSize) && bigFileSize > 0)
        {
            _defaultBigFileSize = bigFileSize;
        }
    }

    /// <summary>
    /// Dowloads file from database.
    /// </summary>
    /// <param name="userDataId"></param>
    /// <param name="fileId"></param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(UserFileStreamDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet]
    [Route("Download")]
    public async Task<IActionResult> DownloadFile([FromQuery] int userDataId, [FromQuery] int fileId,
                CancellationToken cancellationToken = default)
    {
        var timer = new Stopwatch();
        timer.Start();

        ResultWrapper<UserFileStreamDto> ret = await _service.DownloadFileAsync(GetCurrentUser(), userDataId, fileId, cancellationToken);
        if (!ret.Success)
        {
            return StatusCode(ret.StatusCode);
        }

        new FileExtensionContentTypeProvider()
                        .TryGetContentType(ret.Data!.Name, out string? contentType);

        timer.Stop();
        Console.WriteLine($"DownloadFile {ret.Data.Name}: {timer.Elapsed.Minutes}:{timer.Elapsed.Seconds}, Length:{ret.Data.Size}");

        return File(ret.Data.Content!, contentType ?? "application/octet-stream", ret.Data.Name);
    }

    /// <summary>
    /// Returns list of all files in database for UserDataDto"/>.
    /// </summary>
    /// <param name="userDataId"></param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>Array of files <see cref="UserFileDto"/></returns>
    [HttpGet]
    [ProducesResponseType(typeof(UserFileDto[]), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetList([FromQuery] int userDataId, CancellationToken cancellationToken = default)
    {
        ResultWrapper<UserFileDto[]> ret = await _service.GetListAsync(GetCurrentUser(), userDataId, cancellationToken);

        if (!ret.Success)
        {
            return StatusCode(ret.StatusCode);
        }

        return Ok(ret.Data);
    }

    /// <summary>
    /// Deletes file from database.
    /// </summary>
    /// <param name="userDataId"></param>
    /// <param name="fileId"></param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>Id of deleted file</returns>
    [HttpDelete]
    public async Task<IActionResult> DeleteFile([FromQuery] int userDataId, [FromQuery] int fileId,
                CancellationToken cancellationToken = default)
    {
        ResultWrapper<int> ret = await _service.DeleteFileAsync(GetCurrentUser(), userDataId, fileId, cancellationToken);

        if (!ret.Success)
        {
            return StatusCode(ret.StatusCode);
        }

        return Ok(ret.Data);
    }

    /// <summary>
    /// Uploads file to database. Multipart/form-data is used. If there are several form-data only the first one will be processed.
    /// </summary>
    /// <param name="userDataId">Id of User Data</param>
    /// <param name="fileId">Id of file</param>
    /// <param name="bigFile">Boolean flag ("true"/"false"). If "true", special feature of storing of big files to database is used.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns><see cref="UserFile"/></returns>
    [HttpPost]
    [Route("Upload")]
    [DisableRequestSizeLimit]
    [DisableFormValueModelBinding]
    [ProducesResponseType(typeof(UserFileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadFile(
        [FromQuery] int userDataId, [FromQuery] int fileId, [FromQuery] string? bigFile,
        CancellationToken cancellationToken = default)
    {
        var timer = new Stopwatch();
        timer.Start();

        Console.WriteLine($"Request length: {Request.ContentLength}");

        if (!bool.TryParse(Request.Query["BigFile"], out bool flagBigFile))
        {
            flagBigFile = AutoDetectBigFile();
        }

        string? boundary = HeaderUtilities.RemoveQuotes(
            MediaTypeHeaderValue.Parse(Request.ContentType).Boundary
        ).Value;

        var reader = new MultipartReader(boundary!, Request.Body, _defaultBufferSize);

        var file = new UserFile { Id = fileId, UserDataId = userDataId };
        ResultWrapper<UserFileDto> result = await UploadFile(reader, file, flagBigFile, cancellationToken);

        if (!result.Success)
        {
            return StatusCode(result.StatusCode);
        }

        timer.Stop();
        Console.WriteLine($"DownloadFile {result.Data!.Name}: {timer.Elapsed.Minutes}:{timer.Elapsed.Seconds}, Length:{result.Data.Size}");

        return Ok(result.Data);
    }

    private async Task<ResultWrapper<UserFileDto>> UploadFile(MultipartReader reader, UserFile file, bool flagBigFile,
        CancellationToken cancellationToken)
    {
        ResultWrapper<UserFileDto> uploadedFile = new();  // result

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
                    BigFile = flagBigFile,
                    Name = contentDisposition.FileName.Value!
                };

                string? tmpFile = null;

                try
                {
                    Stream? uploadStream = null;

                    if (_useTemporaryFile)
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

                    uploadedFile = await _service.UploadFileAsync(GetCurrentUser(), uploadData, cancellationToken);
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

    #region Helpers

    private static async Task<string> CopyStreamToFile(Stream inputStream, string fileName)
    {
        DirectoryInfo tempDir = Directory.CreateTempSubdirectory("DataManagerAPI");
        string fullName = Path.Combine(tempDir.FullName, fileName);

        await using var outStream = new System.IO.FileStream(fullName, FileMode.CreateNew);
        await inputStream.CopyToAsync(outStream);

        return fullName;
    }

    private bool AutoDetectBigFile()
    {
        int lengthMB = Convert.ToInt32((Request.ContentLength! / 1024f / 1024f));
        return lengthMB >= _defaultBigFileSize;
    }

    /// <summary>
    /// Gets current user from HttpContext.
    /// </summary>
    /// <returns><see cref="CurrentUserDto"/></returns>
    private CurrentUserDto? GetCurrentUser()
    {
        return HttpContext.Items["User"] as CurrentUserDto;
    }

    #endregion
}
