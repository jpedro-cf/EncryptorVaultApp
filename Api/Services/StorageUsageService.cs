using Microsoft.EntityFrameworkCore;
using MyMVCProject.Api.Entities;
using MyMVCProject.Config;

namespace MyMVCProject.Api.Services;

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
}