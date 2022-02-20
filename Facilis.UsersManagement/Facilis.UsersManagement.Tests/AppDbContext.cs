using Facilis.UsersManagement.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Facilis.UsersManagement.Tests
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        #region Constructor(s)

        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        #endregion Constructor(s)
    }
}