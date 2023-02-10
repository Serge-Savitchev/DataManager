namespace DataManagerAPI.Dto;

public class UserFileDto
{
    /// <summary>
    /// Key.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Id of User Data
    /// </summary>
    public int UserDataId { get; set; }

    /// <summary>
    /// File name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Size of file in bytes.
    /// </summary>
    public long Size { get; set; }
}
