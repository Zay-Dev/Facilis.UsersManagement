using Facilis.UsersManagement.Abstractions;
using Facilis.UsersManagement.Enums;
using Facilis.UsersManagement.Tests.Helpers;
using Facilis.UsersManagement.Tests.Services;
using NUnit.Framework;
using System;

namespace Facilis.UsersManagement.Tests
{
    public class TokenAuthenticationFailureType
    {
        public const string USERNAME = nameof(USERNAME);
        public const string PASSWORD = nameof(PASSWORD);

        private Instances instances { get; set; }
        private IAuthenticator<ITokenBase, User> authenticator { get; set; }

        [SetUp]
        public void Setup()
        {
            this.instances = new Instances(new BCryptNetPasswordHasher());
            this.authenticator = new TokenBasedAuthenticator<UserToken, User>(
                this.instances.Users,
                this.instances.UserTokens,
                new PlainPasswordHashService()
            );
        }

        [TearDown]
        public void TearDown()
        {
            this.instances.Dispose();
        }

        [Test]
        public void TestAuthenticate_FailureTypeNone()
        {
            // Arrange
            var user = this.instances
                .PasswordHasher
                .CreateUser(USERNAME, PASSWORD);
            var token = new PlainPasswordHashService().CreateUserToken(user);

            token.ExpiredAtUtc = DateTime.MaxValue;
            this.instances.Users.Add(user);
            this.instances.UserTokens.Add(token);

            // Act
            var failure = this.authenticator
                .TryAuthenticate(GetAuthenticateInput(token.Id, token.HashedPassword))
                .Failure;

            // Assert
            Assert.AreEqual(LoginFailureTypes.None, failure);
            Assert.Pass();
        }

        [Test]
        public void TestAuthenticate_FailureTypeTokenIsExpired()
        {
            // Arrange
            var user = this.instances
                .PasswordHasher
                .CreateUser(USERNAME, PASSWORD);
            var token = new PlainPasswordHashService().CreateUserToken(user);

            token.ExpiredAtUtc = DateTime.MinValue;
            this.instances.Users.Add(user);
            this.instances.UserTokens.Add(token);

            // Act
            var failure = this.authenticator
                .TryAuthenticate(GetAuthenticateInput(token.Id, token.HashedPassword))
                .Failure;

            // Assert
            Assert.AreEqual(LoginFailureTypes.TokenIsExpired, failure);
            Assert.Pass();
        }

        [Test]
        public void TestAuthenticate_FailureTypeTokenNotFound()
        {
            // Arrange
            var user = this.instances
                .PasswordHasher
                .CreateUser(USERNAME, PASSWORD);

            this.instances.Users.Add(user);

            // Act
            var failure = this.authenticator
                .TryAuthenticate(GetAuthenticateInput(
                    $"{Guid.NewGuid()}",
                    $"{Guid.NewGuid()}"
                ))
                .Failure;

            // Assert
            Assert.AreEqual(LoginFailureTypes.TokenNotFound, failure);
            Assert.Pass();
        }

        [Test]
        public void TestAuthenticate_FailureTypeTokenMismatch()
        {
            // Arrange
            var user = this.instances
                .PasswordHasher
                .CreateUser(USERNAME, PASSWORD);
            var token = new PlainPasswordHashService().CreateUserToken(user);

            token.ExpiredAtUtc = DateTime.MaxValue;
            this.instances.Users.Add(user);
            this.instances.UserTokens.Add(token);

            // Act
            var failure = this.authenticator
                .TryAuthenticate(GetAuthenticateInput(token.Id, $"{Guid.NewGuid()}"))
                .Failure;

            // Assert
            Assert.AreEqual(LoginFailureTypes.TokenMismatch, failure);
            Assert.Pass();
        }

        private static TokenBase GetAuthenticateInput(
            string tokenId,
            string value
        )
        {
            return new()
            {
                TokenId = tokenId,
                Value = value,
            };
        }
    }
}