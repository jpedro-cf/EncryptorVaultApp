using EncryptionApp.Api.Dtos.Items;
using EncryptionApp.Api.Factory;
using EncryptionApp.Api.Global;
using EncryptionApp.Config;
using Microsoft.EntityFrameworkCore;

namespace EncryptionApp.Api.Services;

public class ItemsService(AppDbContext ctx, ItemResponseFactory responseFactory)
{
    public async Task<Result<List<ItemResponse>>> GetAll(Guid userId)
    {
        var items = await ctx.Folders
            .Where(f => f.ParentFolderId == null && f.OwnerId == userId)
            .Select(item => ItemResponse.From(item, true))
            .ToListAsync();

        var files = await Task.WhenAll(
            ctx.Files.Where(f => f.ParentFolderId == null && f.OwnerId == userId)
                .Select(f => responseFactory.CreateFrom(f, true)));
        
        items.AddRange(files);

        return Result<List<ItemResponse>>.Success(items);
    }
}