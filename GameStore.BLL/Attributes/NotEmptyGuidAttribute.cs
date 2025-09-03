using System.ComponentModel.DataAnnotations;

namespace GameStore.BLL.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class NotEmptyGuidAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        return value is Guid guid && guid != Guid.Empty;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} cannot be an empty GUID.";
    }
}