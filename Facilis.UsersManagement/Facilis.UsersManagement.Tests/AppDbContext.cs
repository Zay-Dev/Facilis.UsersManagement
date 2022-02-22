using Facilis.UsersManagement.Abstractions;
using Facilis.UsersManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace Facilis.UsersManagement.Tests
{
    public class AppDbContext : DbContext
    {
        public DbSet<User<UserProfile>> Users { get; set; }

        #region Constructor(s)

        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        #endregion Constructor(s)
    }
}