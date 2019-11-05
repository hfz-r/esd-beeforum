using System.Data;
using beeforum.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace beeforum.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        private IDbContextTransaction _dbContextTransaction;

        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Article> Articles { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ArticleTag> ArticleTags { get; set; }
        public DbSet<ArticleFavorite> ArticleFavorites { get; set; }
        public DbSet<FollowedPeople> FollowedPeople { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ArticleTag>(builder =>
            {
                builder.HasKey(t => new { t.ArticleId, t.TagId });

                builder.HasOne(pt => pt.Article)
                .WithMany(p => p.ArticleTags)
                .HasForeignKey(pt => pt.ArticleId);

                builder.HasOne(pt => pt.Tag)
                .WithMany(t => t.ArticleTags)
                .HasForeignKey(pt => pt.TagId);
            });

            modelBuilder.Entity<ArticleFavorite>(builder =>
            {
                builder.HasKey(t => new { t.ArticleId, t.PersonId });

                builder.HasOne(pt => pt.Article)
                    .WithMany(p => p.ArticleFavorites)
                    .HasForeignKey(pt => pt.ArticleId);

                builder.HasOne(pt => pt.Person)
                    .WithMany(t => t.ArticleFavorites)
                    .HasForeignKey(pt => pt.PersonId);
            });

            modelBuilder.Entity<FollowedPeople>(builder =>
            {
                builder.HasKey(t => new { t.ObserverId, t.TargetId });

                builder.HasOne(pt => pt.Observer)
                    .WithMany(p => p.Following)
                    .HasForeignKey(pt => pt.ObserverId)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(pt => pt.Target)
                    .WithMany(t => t.Followers)
                    .HasForeignKey(pt => pt.TargetId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        #region Transaction handler

        public void BeginTransaction()
        {
            if (_dbContextTransaction != null)
                return;

            if (!Database.IsInMemory())
                _dbContextTransaction = Database.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        public void CommitTransaction()
        {
            try
            {
                _dbContextTransaction?.Commit();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_dbContextTransaction != null)
                {
                    _dbContextTransaction.Dispose();
                    _dbContextTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _dbContextTransaction?.Rollback();
            }
            finally 
            {
                if (_dbContextTransaction != null)
                {
                    _dbContextTransaction.Dispose();
                    _dbContextTransaction = null;
                }
            }
        }

        #endregion
    }
}