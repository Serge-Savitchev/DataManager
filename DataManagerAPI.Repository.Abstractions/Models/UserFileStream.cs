using System.Runtime.Serialization;

namespace DataManagerAPI.Repository.Abstractions.Models;

/// <summary>
/// Class for upload/download file to/from database.
/// </summary>
[DataContract]
public class UserFileStream : UserFile
{
    /// <summary>
    /// Bynary stream with file content.
    /// </summary>
    [DataMember(Order = 5)]
    public Stream? Content { get; set; }

    /// <summary>
    /// Sign of big file.
    /// </summary>
    [DataMember(Order = 6)]
    public bool BigFile { get; set; }
}
