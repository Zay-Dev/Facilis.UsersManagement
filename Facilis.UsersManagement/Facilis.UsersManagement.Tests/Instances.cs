using Facilis.Core.Abstractions;
using Facilis.Core.EntityFrameworkCore;
using Facilis.UsersManagement.Abstractions;
using Facilis.UsersManagement.Models;
using Facilis.UsersManagement.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using System;

namespace Facilis.UsersManagement.Tests
{
    public class Instances : IDisposable
    {
        public IPasswordHasher PasswordHasher { get; }

        public DbContext Context { get; }
        public IEntities<User<UserProfile>> Users { get; }
        public IAuthenticator Authenticator { get; }

        #region Constructor(s)

        public Instances(IPasswordHasher passwordHasher)
        {
            this.PasswordHasher = passwordHasher;

            this.Context = nameof(Facilis).InMemoryContext<AppDbContext>();
            this.Users = new Entities<User<UserProfile>>(this.Context);
            this.Authenticator = new Authenticator<User<UserProfile>>(
                this.Users,
                this.PasswordHasher
            );
        }

        #endregion Constructor(s)

        public void Dispose()
        {
            this.Context.Database.EnsureDeleted();
            this.Users.Dispose();
        }
    }
}