using Drippy_Dropper.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using File = Drippy_Dropper.API.Models.Entities.File;

namespace Drippy_Dropper.API.Models.Context
{
    public class DrippyDropperDb : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Folder> Folders { get; set; } = null!;
        public DbSet<File> Files { get; set; } = null!;

        public DrippyDropperDb(DbContextOptions<DrippyDropperDb> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
            });

            modelBuilder.Entity<Folder>(entity =>
            {
                entity.HasKey(e => e.FolderId);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Path).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();

                entity.HasOne(e => e.ParentFolder)
                    .WithMany(e => e.SubFolders)
                    .HasForeignKey(e => e.ParentFolderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<User>()
                    .WithMany(u => u.Folders)
                    .HasForeignKey(e => e.OwnerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<File>(entity =>
            {
                entity.HasKey(e => e.FileId);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.ContentType).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Path).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();

                entity.HasOne(e => e.Folder)
                    .WithMany(f => f.Files)
                    .HasForeignKey(e => e.FolderId)
                    .OnDelete(DeleteBehavior.Restrict);


                entity.HasOne<User>()
                    .WithMany(u => u.Files)
                    .HasForeignKey(e => e.OwnerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
