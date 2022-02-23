using Facilis.Core.Abstractions;
using Facilis.UsersManagement.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Facilis.UsersManagement.SampleApp
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<ExtendedAttribute> ExtendedAttributes { get; set; }

        #region Constructor(s)

        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        #endregion Constructor(s)

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>()
                .HasIndex(user => user.Username)
                .IsUnique();
            base.OnModelCreating(builder);
        }
    }
}