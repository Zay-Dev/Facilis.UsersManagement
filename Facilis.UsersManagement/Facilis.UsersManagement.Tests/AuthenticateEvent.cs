using Facilis.Core.Abstractions;
using Facilis.Core.EntityFrameworkCore;
using Facilis.UsersManagement.Abstractions;
using Facilis.UsersManagement.Enums;
using Facilis.UsersManagement.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;

namespace Facilis.UsersManagement.Tests
{
    public class AuthenticateEvent
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
        public void TestAuthenticated_EventTriggered()
        {
            // Arrange
            var triggered = false;
            var user = this.passwordHasher.CreateUser(USERNAME, PASSWORD);
            this.users.Add(user);

            // Act
            this.authenticator.Authenticated += (_, user) =>
            {
                Console.WriteLine(user.Id);
                triggered = true;
            };
            this.authenticator
                .TryAuthenticate(USERNAME, PASSWORD, out var _);

            // Assert
            Assert.IsTrue(triggered);
            Assert.Pass();
        }

        [Test]
        public void TestAuthenticateFailed_EventTriggered()
        {
            // Arrange
            var failureType = LoginFailureTypes.None;

            // Act
            this.authenticator.AuthenticateFailed += (_, type, username, password) =>
            {
                Console.WriteLine($"{username} | {password}");
                failureType = type;
            };
            this.authenticator
                .TryAuthenticate(USERNAME, PASSWORD, out var _);

            // Assert
            Assert.AreNotEqual(LoginFailureTypes.None, failureType);
            Assert.Pass();
        }
    }
}