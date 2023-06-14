using Dcis.Amfi.Isf.Mock.Constants;
using Dcis.Amfi.Isf.Mock.Extensions;
using Dcis.Amfi.Isf.Mock.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace Dcis.Amfi.Isf.Mock.Helpers
{
    public static class JwtHelper
    {
        const string Kid = "kid";
        const string LocalHost = "localhost";
        const string GateWay = "Gateway{InetSSL=5}";
        const string JwtCertificateKey = "JwtCertificate:Certificate";
        const string JwtCertPasswordKey = "JwtCertificate:Password";

        public static string GenerateToken(User user, IConfiguration configuration)
        {
            if(user == null)
                return string.Empty;

            SigningCredentials credential = GetCredential(configuration);
            JwtSecurityTokenHandler jwtTokenHandler = new();
            ClaimsIdentity claims = new();

            claims.AddUserClaims(UserClaimHelper.GenerateUserClaims(user));
            claims.AddClaim(new Claim(UserClaimTypes.IsfRequestId, Guid.NewGuid().ToString("N")));
            claims.AddClaim(new Claim(UserClaimTypes.IsfSessionId, Guid.NewGuid().ToString("N")));
            claims.AddClaim(new Claim(UserClaimTypes.IsfGateWay, GateWay));
            claims.AddClaim(new Claim(UserClaimTypes.IsfHops, LocalHost));
            claims.AddClaim(new Claim(UserClaimTypes.InternalIdentityId, user.InternalIdentityId));
            claims.AddClaim(new Claim(UserClaimTypes.AtoInternalIdentityId, user.InternalIdentityId));

            if (!string.IsNullOrWhiteSpace(user.OperatorInternalId))
            {
                claims.AddClaim(new Claim(UserClaimTypes.OperatorInternalId, user.OperatorInternalId));
            }

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Audience = user.Audience,
                Expires = DateTime.Now.AddMinutes(60),
                Issuer = user.Issuer,
                NotBefore = DateTime.Now.AddMinutes(-1),
                Subject = claims,
                SigningCredentials = credential,
                IssuedAt = DateTime.Now
            };

            JwtSecurityToken secToken = jwtTokenHandler.CreateJwtSecurityToken(tokenDescriptor);

            secToken.Header.Remove(Kid);

            return jwtTokenHandler.WriteToken(secToken); 
        }

        private static SigningCredentials GetCredential(IConfiguration configuration)
        {
            // Certificate needs to be change to align with the public key of PIP api
            X509Certificate2 certificate = new(configuration.GetValue<string>(JwtCertificateKey), configuration.GetValue<string>(JwtCertPasswordKey));

            X509SecurityKey secKey = new(certificate);

            return new SigningCredentials(secKey, SecurityAlgorithms.RsaSha256Signature);
        }
    }
}
