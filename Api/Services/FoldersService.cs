using Microsoft.EntityFrameworkCore;
using MyMVCProject.Api.Dtos.Folders;
using MyMVCProject.Api.Entities;
using MyMVCProject.Api.Infra.Security;
using MyMVCProject.Config;

namespace MyMVCProject.Api.Services;

public class FoldersService(AppDbContext ctx, UsersService usersService)
{
    public async Task<CreateFolderResponse> Create(Guid userId, CreateFolderRequest data)
    {
        var encryptionKey = EncryptionHandler.GenerateRandomAes256Key(); // already base64

        if (data.ParentId is null)
        {
            var (salt, generatedKey) = KeyDerivationHandler.CreateDerivedKey(data.Secret!);
            
            var keyEncrypted = EncryptionHandler.Encrypt(encryptionKey, generatedKey);
            
            var folder = Folder.CreateRoot(data.Name, userId, keyEncrypted, salt);

            ctx.Folders.Add(folder);
            await ctx.SaveChangesAsync();

            return new CreateFolderResponse(FolderResponse.From(folder), generatedKey, generatedKey);
        }

        var parent = await ctx.Folders.FirstAsync(f => f.Id == data.ParentId!);

        // Verify if keys provided are correct by trying to decrypt them.
        // If is not correct, it will throw an error internally
        var parentKey = EncryptionHandler.Decrypt(parent.EncryptedKey, data.EncryptionKey!);
        var parentRootKey = EncryptionHandler.Decrypt(parent.KeyEncryptedByRoot, data.RootEncryptionKey!);
        
        var encryptedKey = EncryptionHandler.Encrypt(encryptionKey, parentKey);
        var encryptedKeyByRoot = EncryptionHandler.Encrypt(encryptionKey, data.RootEncryptionKey!);

        var subFolder = Folder.CreateSubFolder(
            data.Name,
            userId,
            data.ParentId!.Value,
            encryptedKey,
            encryptedKeyByRoot,
            parent.RootKeySalt);

        ctx.Folders.Add(subFolder);
        await ctx.SaveChangesAsync();
        
        return new CreateFolderResponse(FolderResponse.From(subFolder), parentKey, data.RootEncryptionKey!);
    }
    
    
    public bool CanBeViewedByUser(Guid userId, Folder folder)
    {
        return folder.OwnerId.Equals(userId);
    }
}