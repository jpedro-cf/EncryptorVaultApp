using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyMVCProject.Api.Entities;

public class Folder : BaseEncryptedEntity
{
    public Guid? ParentFolderId { get; set; }
    public virtual ICollection<Folder> Folders { get; set; }
    
    public virtual ICollection<File> Files { get; set; }

    public static Folder CreateRoot(string name, Guid ownerId, string encryptedKey, string keyEncryptedByRoot)
    {
        return new Folder
        {
            Name = name,
            OwnerId = ownerId,
            EncryptedKey = encryptedKey,
            KeyEncryptedByRoot = keyEncryptedByRoot,
        };
    }

    public static Folder CreateSubFolder(
        string name, 
        Guid ownerId,
        Guid parentFolderId,
        string encryptedKey, 
        string keyEncryptedByRoot)
    {
        return new Folder
        {
            Name = name,
            OwnerId = ownerId,
            ParentFolderId = parentFolderId,
            EncryptedKey = encryptedKey,
            KeyEncryptedByRoot = keyEncryptedByRoot
        };
    }
}