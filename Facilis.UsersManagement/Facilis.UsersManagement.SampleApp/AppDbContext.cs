using Facilis.UsersManagement.Abstractions;
using Facilis.UsersManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace Facilis.UsersManagement.SampleApp
{
    public class AppDbContext : DbContext
    {
        public DbSet<User<UserProfile>> Users { get; set; }

        #region Constructor(s)

        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        #endregion Constructor(s)

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User<UserProfile>>()
                .HasIndex(user => user.Username)
                .IsUnique();
            base.OnModelCreating(builder);
        }
    }
}