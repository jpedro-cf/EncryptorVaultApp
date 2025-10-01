using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyMVCProject.Api.Entities;

public class Folder : BaseEncryptedEntity
{
    public Guid? ParentFolderId { get; set; }
    public ICollection<Folder> Folders { get; set; }
    
    public ICollection<File> Files { get; set; }

    public bool IsRoot()
    {
        return ParentFolderId is null;
    }

    public static Folder CreateRoot(string name, Guid ownerId, string encryptedKey, string rootKeySalt)
    {
        return new Folder
        {
            Name = name,
            OwnerId = ownerId,
            EncryptedKey = encryptedKey,
            KeyEncryptedByRoot = encryptedKey,
            RootKeySalt = rootKeySalt
        };
    }

    public static Folder CreateSubFolder(
        string name, 
        Guid ownerId,
        Guid parentFolderId,
        string encryptedKey, 
        string keyEncryptedByRoot, 
        string rootKeySalt)
    {
        return new Folder
        {
            Name = name,
            OwnerId = ownerId,
            ParentFolderId = parentFolderId,
            EncryptedKey = encryptedKey,
            KeyEncryptedByRoot = keyEncryptedByRoot,
            RootKeySalt = rootKeySalt
        };
    }
}