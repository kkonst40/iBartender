using iBartender.Persistence.Entities;
using Microsoft.EntityFrameworkCore;


namespace iBartender.Persistence
{
    public class BartenderDbContext : DbContext
    {
        public BartenderDbContext(DbContextOptions<BartenderDbContext> options) : base(options) { }

        public DbSet<UserEntity> Users { get; set; }
        public DbSet<PublicationEntity> Publications { get; set; }
        public DbSet<UserPublicationEntity> UserPublications { get; set; }
        public DbSet<UserSubscriberEntity> UserSubscribers { get; set; }
        public DbSet<CommentEntity> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserEntity>(entity =>
            {
                entity.HasKey(e => e.Id);

                // One-to-many: User to Publications
                entity.HasMany(u => u.Publications)
                      .WithOne(p => p.User)
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                // One-to-many: User to Comments
                entity.HasMany(u => u.Comments)
                      .WithOne(c => c.User)
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // PublicationEntity configuration
            modelBuilder.Entity<PublicationEntity>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Many-to-one: Publication to User
                entity.HasOne(p => p.User)
                      .WithMany(u => u.Publications)
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                // One-to-many: Publication to Comments
                entity.HasMany(p => p.Comments)
                      .WithOne(c => c.Publication)
                      .HasForeignKey(c => c.PublicationId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserPublicationEntity>(entity =>
            {
                entity.HasKey(up => new { up.UserId, up.PublicationId });

                // Many-to-one: Like to User
                entity.HasOne(up => up.User)
                      .WithMany(u => u.LikedPublications)
                      .HasForeignKey(up => up.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Many-to-one: Like to Publication
                entity.HasOne(up => up.Publication)
                      .WithMany(p => p.LikedPublications)
                      .HasForeignKey(up => up.PublicationId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserSubscriberEntity>(entity =>
            {
                entity.HasKey(us => new { us.UserId, us.SubscriberId });

                // Many-to-one: Subscription to User (who is being subscribed to)
                entity.HasOne(us => us.User)
                      .WithMany(u => u.Subscribers)
                      .HasForeignKey(us => us.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Many-to-one: Subscription to Subscriber (who is subscribing)
                entity.HasOne(us => us.Subscriber)
                      .WithMany(u => u.Subscriptions)
                      .HasForeignKey(us => us.SubscriberId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<CommentEntity>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Many-to-one: Comment to User
                entity.HasOne(c => c.User)
                      .WithMany(u => u.Comments)
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Many-to-one: Comment to Publication
                entity.HasOne(c => c.Publication)
                      .WithMany(p => p.Comments)
                      .HasForeignKey(c => c.PublicationId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}