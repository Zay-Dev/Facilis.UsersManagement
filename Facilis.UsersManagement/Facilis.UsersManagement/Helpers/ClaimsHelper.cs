using Facilis.UsersManagement.Abstractions;
using Facilis.UsersManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Facilis.UsersManagement.Helpers
{
    public static class ClaimsHelper
    {
        public static string GetValueOfClaimType(this IEnumerable<Claim> claims, string type)
        {
            return claims.FirstOrDefault(claim => claim.Type == type)?.Value;
        }

        public static Claim[] GetRoles(this IEnumerable<Claim> claims)
        {
            return claims.Where(claim => claim.Type == ClaimTypes.Role).ToArray();
        }

        public static Claim[] GetRoles(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.Claims.GetRoles();
        }

        public static Claim GetIdentifier(this IEnumerable<Claim> claims)
        {
            return claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);
        }

        public static Claim GetUsername(this IEnumerable<Claim> claims)
        {
            return claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name);
        }

        public static Claim GetIdentifier(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.Claims.GetIdentifier();
        }

        public static Claim GetUsername(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.Claims.GetUsername();
        }

        public static bool IsInRole(
            this ClaimsPrincipal claimsPrincipal,
            Enum role
        )
        {
            return claimsPrincipal.IsInRole(role.ToString());
        }

        public static bool IsInAnyOfRoles(
            this ClaimsPrincipal claimsPrincipal,
            params Enum[] roles
        )
        {
            return roles.Any(role => claimsPrincipal.IsInRole(role));
        }

        public static Claim[] ToClaims(this IUser<UserProfile> user)
        {
            return new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(nameof(user.Profile.Nickname), user.Profile.Nickname ?? ""),
                new Claim(ClaimTypes.Email, user.Profile.Email ?? ""),
                new Claim(ClaimTypes.GivenName, user.Profile.FirstName ?? ""),
                new Claim(ClaimTypes.Surname, user.Profile.LastName ?? ""),
            }
                .Concat(user.Profile
                    .Roles
                    .Select(role => new Claim(ClaimTypes.Role, role))
                )
                .ToArray();
        }

        public static UserProfile ToUserProfile(this IEnumerable<Claim> claims)
        {
            return new UserProfile()
            {
                Nickname = claims.GetValueOfClaimType(nameof(UserProfile.Nickname)),
                Email = claims.GetValueOfClaimType(ClaimTypes.Email),
                FirstName = claims.GetValueOfClaimType(ClaimTypes.GivenName),
                LastName = claims.GetValueOfClaimType(ClaimTypes.Surname),
                Roles = claims.GetRoles().Select(claim => claim.Value).ToArray(),
            };
        }

        public static UserProfile ToUserProfile(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.Claims.ToUserProfile();
        }
    }
}