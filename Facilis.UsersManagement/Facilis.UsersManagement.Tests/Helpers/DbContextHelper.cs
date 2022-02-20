using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Facilis.UsersManagement.Tests.Helpers
{
    public static class DbContextHelper
    {
        public static T InMemoryContext<T>(this string name) where T : DbContext
        {
            return ActivatorUtilities.CreateInstance<T>(
                null,
                new DbContextOptionsBuilder<T>()
                    .UseInMemoryDatabase(databaseName: name)
                    .Options
            );
        }
    }
}