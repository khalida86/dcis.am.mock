using Dcis.Amfi.Isf.Mock.Constants;
using Dcis.Amfi.Isf.Mock.Models;

namespace Dcis.Amfi.Isf.Mock.Helpers
{
    public static class UserClaimHelper
    {
        public static IEnumerable<UserClaim> GenerateUserClaims(User user) => user.JwtType == JwtType.BOIDC ? GenerateBOIDCClaims(user) : GenerateFASClaims(user);

        private static IEnumerable<UserClaim> GenerateBOIDCClaims(User user) {
            List<UserClaim> claims = new()
            {
                new UserClaim { Type = UserClaimTypes.Boidc.Subject, Value = user.Subject },
                new UserClaim { Type = UserClaimTypes.Boidc.Email, Value = user.Email },
                new UserClaim { Type = UserClaimTypes.Boidc.GivenName, Value = user.GivenName },
                new UserClaim { Type = UserClaimTypes.Boidc.FamilyName, Value = user.FamilyName },
                new UserClaim { Type = UserClaimTypes.IsfCredentialType, Value = user.CredentialType }
            };

            if (user.Claims?.Any() == true)
                claims.AddRange(user.Claims);

            return claims;
        }

        private static IEnumerable<UserClaim> GenerateFASClaims(User user)
        {
            List<UserClaim> claims = new()
            {
                new UserClaim { Type = UserClaimTypes.Fas.Subject, Value = user.Subject },
                new UserClaim { Type = UserClaimTypes.Fas.Email, Value = user.Email },
                new UserClaim { Type = UserClaimTypes.Fas.GivenName, Value = user.GivenName },
                new UserClaim { Type = UserClaimTypes.Fas.FamilyName, Value = user.FamilyName },
                new UserClaim { Type = UserClaimTypes.IsfCredentialType, Value = user.CredentialType },
                new UserClaim { Type = UserClaimTypes.AgencyUserId, Value = user.Subject }
            };

            if (user.Claims?.Any() == true)
                claims.AddRange(user.Claims);

            return claims;
        }
    }
}
