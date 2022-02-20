using Facilis.Core.Enums;
using Facilis.UsersManagement.Enums;
using Facilis.UsersManagement.Tests.Helpers;
using NUnit.Framework;
using System;

namespace Facilis.UsersManagement.Tests
{
    public class AuthenticateFailureType
    {
        public const string USERNAME = nameof(USERNAME);
        public const string PASSWORD = nameof(PASSWORD);

        private Instances instances { get; set; }

        [SetUp]
        public void Setup()
        {
            this.instances = new Instances(new BCryptNetPasswordHasher());
        }

        [TearDown]
        public void TearDown()
        {
            this.instances.Dispose();
        }

        [Test]
        public void TestTryAuthenticate_FailureTypeNone()
        {
            // Arrange
            var user = this.instances
                .PasswordHasher
                .CreateUser(USERNAME, PASSWORD);

            this.instances.Users.Add(user);

            // Act
            var failureType = this.instances
                .Authenticator
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
            var failureType = this.instances
                .Authenticator
                .TryAuthenticate(USERNAME, PASSWORD, out var _);

            // Assert
            Assert.AreEqual(LoginFailureTypes.UserNotFound, failureType);
            Assert.Pass();
        }

        [Test]
        public void TestTryAuthenticate_FailureTypeDisabledUser()
        {
            // Arrange
            var user = this.instances
                .PasswordHasher
                .CreateUser(USERNAME, PASSWORD);

            user.Status = StatusTypes.Disabled;
            this.instances.Users.Add(user);

            // Act
            var failureType = this.instances
                .Authenticator
                .TryAuthenticate(USERNAME, PASSWORD, out var _);

            // Assert
            Assert.AreEqual(LoginFailureTypes.DisabledUser, failureType);
            Assert.Pass();
        }

        [Test]
        public void TestTryAuthenticate_FailureTypeLockedUser()
        {
            // Arrange
            var user = this.instances
                .PasswordHasher
                .CreateUser(USERNAME, PASSWORD);

            user.LockedUntilUtc = DateTime.UtcNow.AddMinutes(1);
            this.instances.Users.Add(user);

            // Act
            var failureType = this.instances
                .Authenticator
                .TryAuthenticate(USERNAME, PASSWORD, out var _);

            // Assert
            Assert.AreEqual(LoginFailureTypes.LockedUser, failureType);
            Assert.Pass();
        }

        [Test]
        public void TestTryAuthenticate_FailureTypePasswordMismatch()
        {
            // Arrange
            var user = this.instances
                .PasswordHasher
                .CreateUser(USERNAME, PASSWORD);
            this.instances.Users.Add(user);

            // Act
            var failureType = this.instances
                .Authenticator
                .TryAuthenticate(USERNAME, "", out var _);

            // Assert
            Assert.AreEqual(LoginFailureTypes.PasswordMismatch, failureType);
            Assert.Pass();
        }
    }
}