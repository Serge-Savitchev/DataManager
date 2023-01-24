//namespace Microsoft.AspNetCore.Http;
namespace DataManagerAPI.Repository.Abstractions.Constants;

public static class StatusCodes
{
    /// <summary>
    /// HTTP status code 200.
    /// </summary>
    public const int Status200OK = 200;

    /// <summary>
    /// HTTP status code 201.
    /// </summary>
    public const int Status201Created = 201;

    /// <summary>
    /// HTTP status code 400.
    /// </summary>
    public const int Status400BadRequest = 400;

    /// <summary>
    /// HTTP status code 401.
    /// </summary>
    public const int Status401Unauthorized = 401;

    /// <summary>
    /// HTTP status code 403.
    /// </summary>
    public const int Status403Forbidden = 403;

    /// <summary>
    /// HTTP status code 404.
    /// </summary>
    public const int Status404NotFound = 404;

    /// <summary>
    /// HTTP status code 405.
    /// </summary>
    public const int Status405MethodNotAllowed = 405;

    /// <summary>
    /// HTTP status code 409.
    /// </summary>
    public const int Status409Conflict = 409;

    /// <summary>
    /// HTTP status code 500.
    /// </summary>
    public const int Status500InternalServerError = 500;

    /// <summary>
    /// HTTP status code 501.
    /// </summary>
    public const int Status501NotImplemented = 501;

}
