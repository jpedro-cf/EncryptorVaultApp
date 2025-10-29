using EncryptionApp.Api.Dtos.Items;
using EncryptionApp.Api.Entities;
using EncryptionApp.Api.Global;
using EncryptionApp.Config;
using Microsoft.EntityFrameworkCore;

namespace EncryptionApp.Api.Services;

public class ItemsService(AppDbContext ctx)
{
    public async Task<Result<List<ItemResponse>>> GetAll(Guid userId)
    {
        var items = await ctx.Folders
            .Where(f => f.ParentFolderId == null && f.OwnerId == userId)
            .Select(item => ItemResponse.From(item, true))
            .ToListAsync();

        var files = await ctx.Files
            .Where(f => f.ParentFolderId == null && f.OwnerId == userId && f.Status == FileStatus.Completed)
            .Select(f => ItemResponse.From(f, true))
            .ToListAsync();
        
        items.AddRange(files);

        return Result<List<ItemResponse>>.Success(items);
    }
}