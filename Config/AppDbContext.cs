using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyMVCProject.Api.Entities;
using File = MyMVCProject.Api.Entities.File;

namespace MyMVCProject.Config;

public class AppDbContext(DbContextOptions options) : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Folder> Folders { get; set; }
    public DbSet<File> Files { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<Folder>()
            .HasOne(f => f.Owner)
            .WithMany(u => u.Folders)
            .HasForeignKey(f => f.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<File>()
            .HasOne(f => f.Owner)
            .WithMany(u => u.Files)
            .HasForeignKey(f => f.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Folder>()
            .HasOne<Folder>()
            .WithMany(f => f.Folders)
            .HasForeignKey(f => f.ParentFolderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<File>()
            .HasOne(f => f.ParentFolder)
            .WithMany(f => f.Files)
            .HasForeignKey(f => f.ParentFolderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Folder>()
            .Property(f => f.Name)
            .HasMaxLength(256)
            .IsRequired();

        builder.Entity<File>()
            .Property(f => f.Name)
            .HasMaxLength(256)
            .IsRequired();
        
        builder.Entity<File>()
            .Property(f => f.Status)
            .HasConversion<string>();
    }
}