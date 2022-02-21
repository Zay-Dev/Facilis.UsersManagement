using Facilis.UsersManagement.Enums;
using Facilis.UsersManagement.Tests.Helpers;
using NUnit.Framework;
using System;

namespace Facilis.UsersManagement.Tests
{
    public class AuthenticateEvent
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
        public void TestAuthenticated_EventTriggered()
        {
            // Arrange
            var triggered = false;
            var user = this.instances
                .PasswordHasher
                .CreateUser(USERNAME, PASSWORD);

            this.instances.Users.Add(user);

            // Act
            this.instances.Authenticator.Authenticated += (_, user) =>
            {
                Console.WriteLine(user.Id);
                triggered = true;
            };
            this.instances
                .Authenticator
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
            this.instances
                .Authenticator
                .AuthenticateFailed += (_, type, username, password) =>
                {
                    Console.WriteLine($"{username} | {password}");
                    failureType = type;
                };
            this.instances
                .Authenticator
                .TryAuthenticate(USERNAME, PASSWORD, out var _);

            // Assert
            Assert.AreNotEqual(LoginFailureTypes.None, failureType);
            Assert.Pass();
        }
    }
}