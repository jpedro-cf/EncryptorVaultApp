using EncryptionApp.Api.Dtos.Folders;
using EncryptionApp.Api.Entities;
using EncryptionApp.Api.Global;
using EncryptionApp.Api.Global.Errors;
using EncryptionApp.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EncryptionApp.Api.Services;

public class FoldersService(AppDbContext ctx)
{
    public async Task<Result<FolderResponse>> Create(Guid userId, CreateFolderRequest data)
    {
        if (data.ParentId is null)
        {
            
            var folder = Folder.CreateRoot(data.Name, userId, data.EncryptedKey, data.KeyEncryptedByRoot);

            ctx.Folders.Add(folder);
            await ctx.SaveChangesAsync();

            return Result<FolderResponse>.Success(FolderResponse.From(folder, true));
        }

        var parent = await ctx.Folders.FirstOrDefaultAsync(f => f.Id == data.ParentId);
        if (parent == null)
        {
            return Result<FolderResponse>.Failure(
                new NotFoundError("Parent folder not found."));
        }        

        var subFolder = Folder.CreateSubFolder(
            data.Name,
            userId,
            data.ParentId.Value,
            data.EncryptedKey,
            data.KeyEncryptedByRoot);

        ctx.Folders.Add(subFolder);
        await ctx.SaveChangesAsync();
        
        return Result<FolderResponse>.Success(FolderResponse.From(subFolder, true));
    }

    public async Task<Result<FolderResponse>> GetFolder(Guid folderId, Guid? userId, GetFolderRequest data)
    {
        var folder = await ctx.Folders.FirstOrDefaultAsync(f => f.Id == folderId);
        if (folder == null)
        {
            return Result<FolderResponse>.Failure(new NotFoundError("Folder not found."));
        }
        
        if (!data.ShareId.IsNullOrEmpty())
        {
            var sharedItem = await ctx.SharedItems.FirstOrDefaultAsync(s => 
                s. Id == Guid.Parse(data.ShareId!));
            
            if (sharedItem == null)
            {
                return Result<FolderResponse>.Failure(new NotFoundError("Shared item not found."));
            }
            if (sharedItem.OwnerId != folder.OwnerId)
            {
                return Result<FolderResponse>.Failure(
                    new ForbiddenError("You're not allowed to view this folder."));
            }
            
            return Result<FolderResponse>.Success(FolderResponse.From(folder, false));
        }
        
        if (userId == null)
        {
            return Result<FolderResponse>.Failure(
                new ForbiddenError("You're not allowed to view this folder"));
        }

        return Result<FolderResponse>.Success(FolderResponse.From(folder, true));
    }
}