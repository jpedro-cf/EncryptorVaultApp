using Microsoft.EntityFrameworkCore;
using MyMVCProject.Api.Dtos.Folders;
using MyMVCProject.Api.Entities;
using MyMVCProject.Api.Global;
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

        // Verify if keys provided are correct by trying to decrypt with them.
        // If is not correct, it will throw an error internally
        var parentKey = EncryptionHandler.Decrypt(parent.EncryptedKey, data.EncryptionKey!);
        EncryptionHandler.Decrypt(parent.KeyEncryptedByRoot, data.RootEncryptionKey!);
        
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

    public async Task<GetFolderResponse> GetFolder(Guid folderId, Guid? userId, GetFolderRequest data)
    {
        var folder = await ctx.Folders.FirstAsync(f => f.Id == folderId);
        string decryptedKey;
        
        if (!string.IsNullOrWhiteSpace(data.Secret))
        {
            var (_, key) = KeyDerivationHandler.CreateDerivedKey(folder.RootKeySalt.GetBytes(), data.Secret);

            decryptedKey = EncryptionHandler.Decrypt(folder.KeyEncryptedByRoot, key);

            return new GetFolderResponse(FolderResponse.From(folder), decryptedKey, key);
        }

        decryptedKey = EncryptionHandler.Decrypt(folder.EncryptedKey, data.EncryptionKey!);

        return new GetFolderResponse(FolderResponse.From(folder), decryptedKey, null);
    }
    
    public bool CanBeViewedByUser(Guid userId, Folder folder)
    {
        return folder.OwnerId.Equals(userId);
    }
}