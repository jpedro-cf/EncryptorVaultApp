using EncryptionApp.Api.Dtos.Folders;
using EncryptionApp.Api.Dtos.Items;
using EncryptionApp.Api.Dtos.Share;
using EncryptionApp.Api.Entities;
using EncryptionApp.Api.Global;
using EncryptionApp.Api.Global.Errors;
using EncryptionApp.Config;
using Microsoft.EntityFrameworkCore;

namespace EncryptionApp.Api.Services;

public class ShareService(AppDbContext ctx)
{
    public async Task<Result<SharedLinkResponse>> CreateShare(Guid userId, CreateSharedLinkRequest data)
    {
        object? item = null;
        if (data.Type == ItemType.File)
        {
            item = await ctx.Files.FirstOrDefaultAsync(f => 
                f.OwnerId == userId && f.Id == data.ItemId && f.Status == FileStatus.Completed);
        }
        else
        {
            item = await ctx.Folders.FirstOrDefaultAsync(f => 
                f.OwnerId == userId && f.Id == data.ItemId && f.Status == FolderStatus.Active);
        }
        
        if (item == null)
        {
            return Result<SharedLinkResponse>.Failure(
                new NotFoundError("Item not found."));
        }
        
        var sharedLink = new SharedLink
        {
            OwnerId = userId,
            ItemType = data.Type,
            EncryptedKey = data.EncryptedKey,
            ItemId = data.ItemId
        };
        
        // TODO: Background service to map every sub-files & sub-folders to remove the need of recursive queries
        
        ctx.SharedLinks.Add(sharedLink);
        await ctx.SaveChangesAsync();
        
        return Result<SharedLinkResponse>.Success(SharedLinkResponse.From(sharedLink));
    }

    public async Task<Result<SharedContentResponse>> GetSharedContent(Guid sharedLinkId)
    {
        var sharedLink = await ctx.SharedLinks.FirstOrDefaultAsync(s => s.Id == sharedLinkId);
        if (sharedLink == null)
        {
            return Result<SharedContentResponse>.Failure(new NotFoundError("Shared item not found."));
        }

        if (sharedLink.ItemType == ItemType.File)
        {
            var file = await ctx.Files.FirstOrDefaultAsync(f => 
                f.Id == sharedLink.ItemId && f.Status == FileStatus.Completed);
            
            if (file == null)
            {
                return Result<SharedContentResponse>.Failure(
                    new ForbiddenError("You're not allowed to view this item."));
            }

            var items = new List<ItemResponse>() { ItemResponse.From(file, false) };
            return Result<SharedContentResponse>.Success(
                new SharedContentResponse(items, ItemType.File, EncryptedData.From(sharedLink.EncryptedKey)));
        }
        
        var folder = await ctx.Folders.FirstOrDefaultAsync(f => 
            f.Id == sharedLink.ItemId && f.Status == FolderStatus.Active);
        
        if (folder == null)
        {
            return Result<SharedContentResponse>.Failure(
                new ForbiddenError("You're not allowed to view this item."));
        }

        var children = FolderResponse.From(folder, false).Children;
        return Result<SharedContentResponse>.Success(
            new SharedContentResponse(
                children, 
                ItemType.Folder, 
                EncryptedData.From(sharedLink.EncryptedKey)));
    }

    public async Task<List<SharedLinkResponse>> GetSharedLinks(Guid userId)
    {
        return await ctx.SharedLinks
            .Where(s => s.OwnerId == userId)
            .Select(x => SharedLinkResponse.From(x))
            .ToListAsync();
    }

    public async Task<Result<bool>> DeleteShare(Guid userId, Guid shareId)
    {
        var sharedLink = await ctx.SharedLinks.FirstOrDefaultAsync(s => 
            s.Id == shareId && s.OwnerId == userId);

        if (sharedLink == null)
        {
            return Result<bool>.Failure(
                new NotFoundError("Shared link not found."));
        }

        ctx.SharedLinks.Remove(sharedLink);
        await ctx.SaveChangesAsync();
        
        return Result<bool>.Success(true);
    }
}