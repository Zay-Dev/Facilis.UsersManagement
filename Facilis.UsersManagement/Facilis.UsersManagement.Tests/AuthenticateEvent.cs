using Facilis.UsersManagement.Abstractions;
using Facilis.UsersManagement.Enums;
using Facilis.UsersManagement.Tests.Helpers;
using NUnit.Framework;

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
            var methodName = "";

            var user = this.instances
                .PasswordHasher
                .CreateUser(USERNAME, PASSWORD);
            var input = GetAuthenticateInput(USERNAME, PASSWORD);

            this.instances.Users.Add(user);

            // Act
            this.instances.Authenticator.Authenticated += (_, usedInput, user) =>
            {
                triggered = true;
                methodName = usedInput.MethodName;
            };
            this.instances
                .Authenticator
                .TryAuthenticate(input);

            // Assert
            Assert.IsTrue(triggered);
            Assert.AreEqual(input.MethodName, methodName);
            Assert.Pass();
        }

        [Test]
        public void TestAuthenticateFailed_EventTriggered()
        {
            // Arrange
            var failureType = LoginFailureTypes.None;
            var methodName = "";

            var input = GetAuthenticateInput(USERNAME, PASSWORD);

            // Act
            this.instances
                .Authenticator
                .AuthenticateFailed += (_, usedInput, type) =>
                {
                    failureType = type;
                    methodName = usedInput.MethodName;
                };
            this.instances
                .Authenticator
                .TryAuthenticate(input);

            // Assert
            Assert.AreNotEqual(LoginFailureTypes.None, failureType);
            Assert.AreEqual(input.MethodName, methodName);
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