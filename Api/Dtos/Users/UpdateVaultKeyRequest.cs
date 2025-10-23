using System.ComponentModel.DataAnnotations;

namespace EncryptionApp.Api.Dtos.Users;

public class UpdateVaultKeyRequest
{
    [Required(ErrorMessage = "Vault key is required. Format: Base64(salt + iv + encrypted_root_key)")]
    [Base64String]
    public string VaultKey { get; set; }
}