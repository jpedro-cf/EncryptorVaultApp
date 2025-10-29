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
    public async Task<Result<SharedItemResponse>> CreateShare(Guid userId, CreateSharedItemRequest data)
    {
        object? item = null;
        if (data.Type == ItemType.File)
        {
            item = await ctx.Files.FirstOrDefaultAsync(f => 
                f.OwnerId == userId && f.Id == data.ItemId);
        }
        else
        {
            item = await ctx.Folders.FirstOrDefaultAsync(f => 
                f.OwnerId == userId && f.Id == data.ItemId);
        }
        
        if (item == null)
        {
            return Result<SharedItemResponse>.Failure(
                new NotFoundError("Item not found."));
        }
        
        var sharedItem = new SharedItem
        {
            OwnerId = userId,
            ItemType = data.Type,
            EncryptedKey = data.EncryptedKey,
            ItemId = data.ItemId
        };

        ctx.SharedItems.Add(sharedItem);
        await ctx.SaveChangesAsync();
        
        return Result<SharedItemResponse>.Success(SharedItemResponse.From(sharedItem));
    }

    public async Task<Result<SharedContentResponse>> GetSharedContent(string shareId)
    {
        var sharedItem = await ctx.SharedItems.FirstOrDefaultAsync(s => s.Id == Guid.Parse(shareId));
        if (sharedItem == null)
        {
            return Result<SharedContentResponse>.Failure(new NotFoundError("Shared item not found."));
        }

        if (sharedItem.ItemType == ItemType.File)
        {
            var file = await ctx.Files.FirstOrDefaultAsync(f => f.Id == sharedItem.ItemId);
            if (file == null)
            {
                return Result<SharedContentResponse>.Failure(
                    new NotFoundError("Shared item not found."));
            }

            var items = new List<ItemResponse>() { ItemResponse.From(file, false) };
            return Result<SharedContentResponse>.Success(
                new SharedContentResponse(items, ItemType.File, EncryptedData.From(sharedItem.EncryptedKey)));
        }
        
        var folder = await ctx.Folders.FirstOrDefaultAsync(f => f.Id == sharedItem.ItemId);
        if (folder == null)
        {
            return Result<SharedContentResponse>.Failure(
                new NotFoundError("Shared item not found."));
        }

        var children = FolderResponse.From(folder, false).Children;
        return Result<SharedContentResponse>.Success(
            new SharedContentResponse(children, ItemType.Folder, EncryptedData.From(sharedItem.EncryptedKey)));
    }

    public async Task<List<SharedItemResponse>> GetSharedLinks(Guid userId)
    {
        return await ctx.SharedItems
            .Where(s => s.OwnerId == userId)
            .Select(x => SharedItemResponse.From(x))
            .ToListAsync();
    }

    public async Task<Result<bool>> DeleteShare(Guid userId, Guid shareId)
    {
        var share = await ctx.SharedItems.FirstOrDefaultAsync(s => 
            s.Id == shareId && s.OwnerId == userId);

        if (share == null)
        {
            return Result<bool>.Failure(
                new NotFoundError("Share link not found."));
        }

        ctx.SharedItems.Remove(share);
        await ctx.SaveChangesAsync();
        
        return Result<bool>.Success(true);
    }
}