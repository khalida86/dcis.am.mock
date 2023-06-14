namespace Dcis.Amfi.Isf.Mock.Constants
{
    public class UserClaimTypes
    {
        public const string IsfRequestId = "https://ato.gov.au/isf/request_id/";
        public const string IsfSessionId = "https://ato.gov.au/isf/session_id/";
        public const string IsfGateWay = "https://ato.gov.au/isf/gateway/";
        public const string IsfHops = "https://ato.gov.au/isf/hops/";
        public const string IsfCredentialType = "https://ato.gov.au/isf/credentialtype";
        public const string InternalIdentityId = "urn://au.gov.ato/authorisation/InternalIdentityId";
        public const string AtoInternalIdentityId = "urn://au.gov.ato/authorisation/AtoInternalIdentityId";
        public const string OperatorInternalId = "urn://au.gov.ato/authorisation/OperatorInternalIdentityId";
        public const string AgencyUserId = "https://authorisationmanager.gov.au/identity/claims/agency/userid/agencyUserId";

        public class Boidc
        {
            public const string Subject = "sub";
            public const string Email = "urn://au.gov.ato/authorisation/email";
            public const string GivenName = "urn://au.gov.ato/authentication/DisplayGivenName";
            public const string FamilyName = "urn://au.gov.ato/authentication/DisplayFamilyName";
        }

        public class Fas
        {
            public const string Subject = "sub";
            public const string Email = "email";
            public const string GivenName = "given_name";
            public const string FamilyName = "family_name";
        }

    }
}
