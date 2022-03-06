using Facilis.Core.Abstractions;
using Facilis.Core.EntityFrameworkCore.Helpers;
using Facilis.UsersManagement.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Facilis.UsersManagement.SampleApp
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserToken> UserTokens { get; set; }
        public DbSet<ExtendedAttribute> ExtendedAttributes { get; set; }

        #region Constructor(s)

        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        #endregion Constructor(s)

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasUniqueIndex<User>(user => user.Username);
            builder.HasIndex<ExtendedAttribute>(attribute => new
            {
                attribute.Scope,
                attribute.ScopedId,
                attribute.Key
            });

            this.UseStringifyEnumColumns(builder);
            base.OnModelCreating(builder);
        }
    }
}