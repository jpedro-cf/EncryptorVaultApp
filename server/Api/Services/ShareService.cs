using EncryptionApp.Api.Dtos.Items;
using EncryptionApp.Api.Dtos.Share;
using EncryptionApp.Api.Entities;
using EncryptionApp.Api.Factory;
using EncryptionApp.Api.Global;
using EncryptionApp.Api.Global.Errors;
using EncryptionApp.Config;
using Microsoft.EntityFrameworkCore;

namespace EncryptionApp.Api.Services;

public class ShareService(AppDbContext ctx,  ItemResponseFactory itemResponseFactory)
{
    public async Task<Result<SharedItemResponse>> CreateShare(Guid userId, CreateSharedItemRequest data)
    {
        var user = await ctx.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            return Result<SharedItemResponse>.Failure(
                new UnauthorizedError("You're not allowed to create a share."));
        }
        
        var sharedItem = new SharedItem
        {
            OwnerId = user.Id,
            ItemType = data.Type,
            EncryptedKey = data.EncryptedKey,
            ItemId = data.ItemId
        };

        ctx.SharedItems.Add(sharedItem);
        await ctx.SaveChangesAsync();
        
        return Result<SharedItemResponse>.Success(SharedItemResponse.From(sharedItem));
    }

    public async Task<Result<ItemResponse>> GetSharedContent(string shareId)
    {
        var sharedItem = await ctx.SharedItems.FirstOrDefaultAsync(s => s.Id == Guid.Parse(shareId));
        if (sharedItem == null)
        {
            return Result<ItemResponse>.Failure(new NotFoundError("Shared item not found."));
        }

        if (sharedItem.ItemType == ItemType.File)
        {
            var file = await ctx.Files.FirstOrDefaultAsync(f => f.Id == sharedItem.ItemId);
            if (file == null)
            {
                return Result<ItemResponse>.Failure(
                    new NotFoundError("Item not found."));
            }
            
            return Result<ItemResponse>.Success(
                await itemResponseFactory.CreateFrom(file, false));
        }
        
        var folder = await ctx.Folders.FirstOrDefaultAsync(f => f.Id == sharedItem.ItemId);
        if (folder == null)
        {
            return Result<ItemResponse>.Failure(
                new NotFoundError("Item not found."));
        }
        
        return Result<ItemResponse>.Success(ItemResponse.From(folder, false));
    }

    public async Task<Result<bool>> DeleteShare(Guid userId, Guid shareId)
    {
        var user = await ctx.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            return Result<bool>.Failure(
                new UnauthorizedError("You're not allowed to delete this share."));
        }

        var share = await ctx.SharedItems.FirstOrDefaultAsync(s => 
            s.Id == shareId && s.OwnerId == userId);

        if (share == null)
        {
            return Result<bool>.Failure(
                new NotFoundError("Share not found."));
        }

        ctx.SharedItems.Remove(share);
        await ctx.SaveChangesAsync();
        
        return Result<bool>.Success(true);
    }
}