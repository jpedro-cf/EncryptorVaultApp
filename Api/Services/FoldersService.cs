using EncryptionApp.Api.Dtos.Folders;
using EncryptionApp.Api.Entities;
using EncryptionApp.Api.Global;
using EncryptionApp.Api.Global.Errors;
using EncryptionApp.Config;
using Microsoft.EntityFrameworkCore;

namespace EncryptionApp.Api.Services;

public class FoldersService(AppDbContext ctx, UsersService usersService)
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
        var folder = await ctx.Folders.FirstAsync(f => f.Id == folderId);
        if (userId != null && folder.OwnerId.Equals(userId))
        {
            return Result<FolderResponse>.Failure(new ForbiddenError("You're not allowed to view this folder"));
        }

        bool shouldReturnWithRootKey = userId != null && userId == folder.OwnerId;
        return Result<FolderResponse>.Success(FolderResponse.From(folder, shouldReturnWithRootKey));
    }
}