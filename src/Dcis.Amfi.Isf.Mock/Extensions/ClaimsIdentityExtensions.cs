using Dcis.Amfi.Isf.Mock.Models;
using System.Security.Claims;

namespace Dcis.Amfi.Isf.Mock.Extensions
{
    public static class ClaimsIdentityExtensions
    {
        public static void AddUserClaims(this ClaimsIdentity claimsIdentity, IEnumerable<UserClaim> claims) => claims?.ToList()?.ForEach(c => claimsIdentity.AddUserClaim(c));
        public static void AddUserClaim(this ClaimsIdentity claimsIdentity, UserClaim userClaim) => claimsIdentity.AddUserClaim(userClaim.Type, userClaim.Value);
        public static void AddUserClaim(this ClaimsIdentity claimsIdentity, string type, string value)
        {
            if(!string.IsNullOrWhiteSpace(type) || !string.IsNullOrWhiteSpace(value))
                claimsIdentity.AddClaim(new Claim(type, value));
        }
    }
}
