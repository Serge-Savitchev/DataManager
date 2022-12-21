﻿using DataManagerAPI.Models;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataManagerAPI.Helpers
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class RoleValidationAttribute : ValidationAttribute
    {
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

            bool result = true;

            try
            {
                Enum.Parse<RoleId>(role, true);
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"Invalide value of {name} field. Possible values are: {_availableNames}";
        }

        private readonly static string _availableNames = GetAvailableValues();

        private static string GetAvailableValues()
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (var suit in Enum.GetValues(typeof(RoleId)))
            {
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Append(", ");
                }
                stringBuilder.Append(suit.ToString());
            }

            //stringBuilder.Insert(0, "Invalid role name. Possible values are: ");

            return stringBuilder.ToString();
        }
    }
}
