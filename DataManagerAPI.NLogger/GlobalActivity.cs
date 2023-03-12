using System.Diagnostics;

namespace DataManagerAPI.NLogger;

/// <summary>
/// Class for providing Activity.Current.TraceId value as global item in logs.
/// </summary>
public class GlobalActivity
{
    /// <summary>
    /// Static instance of GlobalActivity.
    /// </summary>
    public static readonly GlobalActivity Default = new();

    /// <summary>
    /// ToString
    /// </summary>
    /// <returns>string representation of Activity.Current.TraceId</returns>
    public override string ToString()
    {
        return Activity.Current == null ? string.Empty : Activity.Current.TraceId.ToString();
    }
}
