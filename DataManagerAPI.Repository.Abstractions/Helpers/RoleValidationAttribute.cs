using DataManagerAPI.Repository.Abstractions.Models;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataManagerAPI.Repository.Abstractions.Helpers;

/// <summary>
/// Role name validator.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class RoleValidationAttribute : ValidationAttribute
{
    /// <summary>
    /// Validates role name.
    /// </summary>
    /// <param name="value">Role name</param>
    /// <returns>True if validation passed</returns>
    public override bool IsValid(object? value)
    {
        if (value == null)
        {
            return false;
        }

        string? role = value as string;
        if (role == null)
        {
            return false;
        }

        if (!Enum.TryParse<RoleIds>(role, true, out _))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Creates error message if validation failed.
    /// </summary>
    /// <param name="name">Role name</param>
    /// <returns>Formatted error message</returns>
    public override string FormatErrorMessage(string name)
    {
        return $"Invalid value of {name} field. Possible values are: {_availableNames}";
    }

    private readonly static string _availableNames = GetAvailableValues();

    private static string GetAvailableValues()
    {
        StringBuilder stringBuilder = new StringBuilder();

        foreach (var suit in RolesHelper.GetAllNames())
        {
            if (stringBuilder.Length > 0)
            {
                stringBuilder.Append(", ");
            }
            stringBuilder.Append(suit.ToString());
        }

        return stringBuilder.ToString();
    }
}
