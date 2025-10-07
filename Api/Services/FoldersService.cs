using Microsoft.EntityFrameworkCore;
using MyMVCProject.Api.Dtos.Folders;
using MyMVCProject.Api.Entities;
using MyMVCProject.Api.Global;
using MyMVCProject.Api.Global.Errors;
using MyMVCProject.Config;

namespace MyMVCProject.Api.Services;

public class FoldersService(AppDbContext ctx, UsersService usersService)
{
    public async Task<Result<FolderResponse>> Create(Guid userId, CreateFolderRequest data)
    {
        if (data.ParentId is null)
        {
            
            var folder = Folder.CreateRoot(data.Name, userId, data.EncryptedKey, data.KeyEncryptedByRoot);

            ctx.Folders.Add(folder);
            await ctx.SaveChangesAsync();

            return Result<FolderResponse>.Success(FolderResponse.From(folder));
        }

        var parent = await ctx.Folders.FirstAsync(f => f.Id == data.ParentId!);
        

        var subFolder = Folder.CreateSubFolder(
            data.Name,
            userId,
            data.ParentId!.Value,
            data.EncryptedKey,
            data.KeyEncryptedByRoot);

        ctx.Folders.Add(subFolder);
        await ctx.SaveChangesAsync();
        
        return Result<FolderResponse>.Success(FolderResponse.From(subFolder));
    }

    public async Task<Result<FolderResponse>> GetFolder(Guid folderId, Guid? userId, GetFolderRequest data)
    {
        var folder = await ctx.Folders.FirstAsync(f => f.Id == folderId);
        if (userId != null && folder.OwnerId.Equals(userId))
        {
            return Result<FolderResponse>.Failure(new ForbiddenError("You're not allowed to view this folder"));
        }
        
        return Result<FolderResponse>.Success(FolderResponse.From(folder));
    }
}