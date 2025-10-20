using EncryptionApp.Api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Entities_File = EncryptionApp.Api.Entities.File;
using File = EncryptionApp.Api.Entities.File;

namespace EncryptionApp.Config;

public class AppDbContext(DbContextOptions options) : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Folder> Folders { get; set; }
    public DbSet<Entities_File> Files { get; set; }
    public DbSet<StorageUsage> StorageUsage { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<Folder>()
            .HasOne(f => f.Owner)
            .WithMany(u => u.Folders)
            .HasForeignKey(f => f.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Entities_File>()
            .HasOne(f => f.Owner)
            .WithMany(u => u.Files)
            .HasForeignKey(f => f.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Folder>()
            .HasOne<Folder>()
            .WithMany(f => f.Folders)
            .HasForeignKey(f => f.ParentFolderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Entities_File>()
            .HasOne(f => f.ParentFolder)
            .WithMany(f => f.Files)
            .HasForeignKey(f => f.ParentFolderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Folder>()
            .Property(f => f.Name)
            .HasMaxLength(256)
            .IsRequired();

        builder.Entity<Entities_File>()
            .Property(f => f.Name)
            .HasMaxLength(256)
            .IsRequired();
        
        builder.Entity<Entities_File>()
            .Property(f => f.Status)
            .HasConversion<string>();

        builder.Entity<StorageUsage>()
            .HasOne(s => s.User)
            .WithMany(u => u.StorageUsages)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Entity<StorageUsage>()
            .Property(s => s.ContentType)
            .HasConversion<string>();
    }
}