using API.Entities;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace API.Data;

public class DataContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<AppUser> Users { get; set; }
    public DbSet<UserLike> Likes { get; set; }
    public DbSet<Message> Messages { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserLike>()
            .HasKey(k => new {k.SourceUserId, k.TargetUserId});

        builder.Entity<UserLike>()
            .HasOne(s => s.SourceUser)
            .WithMany(l => l.LikedUsers)
            .HasForeignKey(s => s.SourceUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<UserLike>()
            .HasOne(s => s.TargetUser)
            .WithMany(l => l.LikedByUsers)
            .HasForeignKey(s => s.TargetUserId)
            .OnDelete(DeleteBehavior.Cascade);


        builder.Entity<Message>()
            .HasOne(m => m.Recipient)
            .WithMany(au => au.MessagesReceived)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany(au => au.MessagesSent)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
