using EncryptionApp.Api.Entities;
using EncryptionApp.Config;
using Microsoft.EntityFrameworkCore;

namespace EncryptionApp.Api.Services;

public class StorageUsageService(AppDbContext ctx)
{
    private readonly long _storageLimit = 256L * 1024 * 1024; // 256MB

    public async Task<bool> StorageLimitExceededWithLock(Guid userId, long newFileSize)
    {
        var usageRows = await ctx.StorageUsage
            .FromSqlInterpolated($@"
                SELECT * FROM ""StorageUsage""
                WHERE ""UserId"" = {userId}
                FOR UPDATE")
            .ToListAsync();
        
        var totalUsed = usageRows.Sum(x => x.TotalSize);

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