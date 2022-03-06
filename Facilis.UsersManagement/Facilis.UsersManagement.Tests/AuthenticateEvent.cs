using Facilis.UsersManagement.Abstractions;
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
                .TryAuthenticate(GetAuthenticateInput(USERNAME, PASSWORD));

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
                .AuthenticateFailed += (_, type, __) =>
                {
                    failureType = type;
                };
            this.instances
                .Authenticator
                .TryAuthenticate(GetAuthenticateInput(USERNAME, PASSWORD));

            // Assert
            Assert.AreNotEqual(LoginFailureTypes.None, failureType);
            Assert.Pass();
        }

        private static PasswordBase GetAuthenticateInput(string username, string password)
        {
            return new()
            {
                Username = username,
                Password = password,
            };
        }
    }
}