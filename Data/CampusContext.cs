using CampusCircleApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusCircleApi.Data
{
    public class CampusContext : DbContext
    {
        public CampusContext(DbContextOptions<CampusContext> options):base(options){}

        public DbSet<User> Users{set;get;}
        public DbSet<Comment> Comments{set;get;}
        public DbSet<Post> Posts {set;get;}
        public DbSet<PostLike> PostLikes{set;get;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PostLike>().HasKey(pl=>new {pl.PostId,pl.UserId});

            modelBuilder.Entity<User>()
            .HasIndex(u=>u.Email)
            .IsUnique();

            modelBuilder.Entity<Post>()
            .HasOne(p=>p.User)
            .WithMany(u=>u.Posts)
            .HasForeignKey(p=>p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
            .HasOne(c=>c.Post)
            .WithMany(p=>p.Comments)
            .HasForeignKey(c=>c.PostId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
            .HasOne(c=>c.User)
            .WithMany()
            .HasForeignKey(c=>c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PostLike>()
            .HasOne(l=>l.User)
            .WithMany(u=>u.UserLikes)
            .HasForeignKey(l=>l.UserId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PostLike>()
            .HasOne(l=>l.Post)
            .WithMany(p=>p.PostLikes)
            .HasForeignKey(l=>l.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        }
    }
}