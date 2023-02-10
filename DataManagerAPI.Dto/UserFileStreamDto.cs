namespace DataManagerAPI.Dto;

/// <summary>
/// Class for upload/download file to/from database.
/// </summary>
public class UserFileStreamDto : UserFileDto
{
    /// <summary>
    /// Bynary stream with file content.
    /// </summary>
    public Stream? Content { get; set; }

    /// <summary>
    /// Sign of big file.
    /// </summary>
    public bool BigFile { get; set; }
}
