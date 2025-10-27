using EncryptionApp.Api.Entities;
using EncryptionApp.Config;
using Microsoft.EntityFrameworkCore;

namespace EncryptionApp.Api.Services;

public class StorageUsageService(AppDbContext ctx)
{
    private readonly long _storageLimit = 10L * 1024 * 1024 * 1024; // 10gb

    public async Task<bool> StorageLimitExceeded(Guid userId, long newFileSize)
    {
        var totalUsed = await ctx.StorageUsage
            .Where(s => s.UserId == userId)
            .SumAsync(s => (long?)s.TotalSize) ?? 0L;

        return totalUsed + newFileSize > _storageLimit;
    }
    
    public async Task<Dictionary<ContentType, long>> GetStorageSummary(Guid userId)
    {
        var summary = await ctx.StorageUsage
            .Where(s => s.UserId == userId)
            .GroupBy(s => s.ContentType)
            .ToDictionaryAsync(
                g => g.Key,
                g => g.Sum(x => x.TotalSize)
            );

        return summary;
    }
}