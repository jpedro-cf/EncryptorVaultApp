using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class PasswordValidation : ValidationAttribute
{
    public int MinimumLength { get; set; } = 8;
    public bool RequireDigit { get; set; } = true;
    public bool RequireLowercase { get; set; } = true;
    public bool RequireUppercase { get; set; } = true;
    public bool RequireNonAlphanumeric { get; set; } = true;
    public int RequiredUniqueChars { get; set; } = 1;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        
        var password = value as string ?? string.Empty;
        
        if (string.IsNullOrWhiteSpace(password))
            return ValidationResult.Success;

        if (password.Length < MinimumLength)
            return new ValidationResult($"Password must be at least {MinimumLength} characters long.");

        if (RequireDigit && !Regex.IsMatch(password, @"\d"))
            return new ValidationResult("Password must contain at least one digit.");

        if (RequireLowercase && !Regex.IsMatch(password, "[a-z]"))
            return new ValidationResult("Password must contain at least one lowercase letter.");

        if (RequireUppercase && !Regex.IsMatch(password, "[A-Z]"))
            return new ValidationResult("Password must contain at least one uppercase letter.");

        if (RequireNonAlphanumeric && !Regex.IsMatch(password, @"[\W_]"))
            return new ValidationResult("Password must contain at least one non-alphanumeric character.");

        if (RequiredUniqueChars > 1 && password.Distinct().Count() < RequiredUniqueChars)
            return new ValidationResult($"Password must contain at least {RequiredUniqueChars} unique characters.");

        return ValidationResult.Success;
    }
}