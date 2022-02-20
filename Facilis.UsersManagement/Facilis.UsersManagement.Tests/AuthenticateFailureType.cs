using Facilis.Core.Abstractions;
using Facilis.Core.EntityFrameworkCore;
using Facilis.Core.Enums;
using Facilis.UsersManagement.Abstractions;
using Facilis.UsersManagement.Enums;
using Facilis.UsersManagement.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;

namespace Facilis.UsersManagement.Tests
{
    public class AuthenticateFailureType
    {
        public const string USERNAME = nameof(USERNAME);
        public const string PASSWORD = nameof(PASSWORD);

        private IPasswordHasher passwordHasher { get; } = new BCryptNetPasswordHasher();

        private DbContext context { get; set; }
        private IEntities<User> users { get; set; }
        private IAuthenticator authenticator { get; set; }

        [SetUp]
        public void Setup()
        {
            this.context = nameof(Facilis).InMemoryContext<AppDbContext>();

            this.users = new Entities<User>(this.context);
            this.authenticator = new Authenticator<User>(this.users, this.passwordHasher);
        }

        [TearDown]
        public void TearDown()
        {
            this.context.Database.EnsureDeleted();
            this.users.Dispose();
        }

        [Test]
        public void TestTryAuthenticate_FailureTypeNone()
        {
            // Arrange
            var user = this.passwordHasher.CreateUser(USERNAME, PASSWORD);
            this.users.Add(user);

            // Act
            var failureType = this.authenticator
                .TryAuthenticate(USERNAME, PASSWORD, out var _);

            // Assert
            Assert.AreEqual(LoginFailureTypes.None, failureType);
            Assert.Pass();
        }

        [Test]
        public void TestTryAuthenticate_FailureTypeUserNotFound()
        {
            // Arrange

            // Act
            var failureType = this.authenticator
                .TryAuthenticate(USERNAME, PASSWORD, out var _);

            // Assert
            Assert.AreEqual(LoginFailureTypes.UserNotFound, failureType);
            Assert.Pass();
        }

        [Test]
        public void TestTryAuthenticate_FailureTypeDisabledUser()
        {
            // Arrange
            var user = this.passwordHasher.CreateUser(USERNAME, PASSWORD);
            user.Status = StatusTypes.Disabled;
            this.users.Add(user);

            // Act
            var failureType = this.authenticator
                .TryAuthenticate(USERNAME, PASSWORD, out var _);

            // Assert
            Assert.AreEqual(LoginFailureTypes.DisabledUser, failureType);
            Assert.Pass();
        }

        [Test]
        public void TestTryAuthenticate_FailureTypeLockedUser()
        {
            // Arrange
            var user = this.passwordHasher.CreateUser(USERNAME, PASSWORD);
            user.LockedUntilUtc = DateTime.UtcNow.AddMinutes(1);
            this.users.Add(user);

            // Act
            var failureType = this.authenticator
                .TryAuthenticate(USERNAME, PASSWORD, out var _);

            // Assert
            Assert.AreEqual(LoginFailureTypes.LockedUser, failureType);
            Assert.Pass();
        }

        [Test]
        public void TestTryAuthenticate_FailureTypePasswordMismatch()
        {
            // Arrange
            var user = this.passwordHasher.CreateUser(USERNAME, PASSWORD);
            this.users.Add(user);

            // Act
            var failureType = this.authenticator
                .TryAuthenticate(USERNAME, "", out var _);

            // Assert
            Assert.AreEqual(LoginFailureTypes.PasswordMismatch, failureType);
            Assert.Pass();
        }
    }
}