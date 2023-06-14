using Dcis.Amfi.Isf.Mock.Helpers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dcis.Amfi.Isf.Mock.Models
{
    public class User
    {
        public string InternalIdentityId { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? OperatorInternalId { get; set; }
        public string Subject { get; set; }
        public string Email { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string CredentialType { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? RedirectUrl { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ClaimAuthUrl { get; set; }
        public JwtType JwtType { get; set; }
        public IEnumerable<UserClaim>? Claims { get; set; }
        public string Audience { get; set; } = "https://am.ato.gov.au/foreigninvestor";
        public string Issuer { get; set; } = "https://ato.gov.au/atoAuthProvider";
        public string DisplayName => $"{GivenName} {FamilyName}";
        private string? _jwtUid;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? JwtUid => _jwtUid;
        public bool HasGeneratedUid => !string.IsNullOrWhiteSpace(_jwtUid);
        public string? JsonClaims
        {
            get
            {
                return Claims != null ? JsonSerializer.Serialize(Claims) : null;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    Claims = JsonSerializer.Deserialize<UserClaim[]>(value)!;
                }
            }
        }

        public void GenerateUid(IConfiguration configuration)
        {
            _jwtUid = JwtHelper.GenerateToken(this, configuration);
        }
    }

    public enum JwtType
    {
        BOIDC,
        FAS
    }
}
