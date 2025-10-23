using System.ComponentModel.DataAnnotations;
using EncryptionApp.Api.Global.Helpers;
using Microsoft.IdentityModel.Tokens;

namespace EncryptionApp.Api.Dtos.Validations;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class ValidContentType: ValidationAttribute
{
   public override bool IsValid(object? value)
   {
      return value is string str && !str.IsNullOrEmpty() && str.IsValidContentType();
   }
}