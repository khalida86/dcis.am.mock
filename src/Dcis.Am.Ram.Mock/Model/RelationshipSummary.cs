using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dcis.Am.Ram.Mock.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class RelationshipSummary
    {
        [JsonProperty("schemas")]
        public string[] schemas { get; set; } = { "urn:id.gov.au:tdif:authorisations:business:1.0" };

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("subjectId")]
        public string SubjectId { get; set; }

        [JsonProperty("subjectIdType")]
        public string SubjectIdType { get; set; }

        [JsonProperty("subjectName")]
        public string SubjectName { get; set; }

        [JsonProperty("relationshipType")]
        public string RelationshipType { get; set; }

        [JsonProperty("startTimestamp")]
        public DateTime? StartTimestamp { get; set; }

        [JsonProperty("endTimestamp")]
        public DateTime? EndTimestamp { get; set; }

        [JsonProperty("attributes")]
        public List<RelationshipSummaryAttribute> Attributes { get; set; }

        [JsonProperty("permissions")]
        public List<string> Permissions { get; set; }

        [JsonProperty("roles")]
        public List<string> Roles { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("lastModified")]
        public DateTime? LastModified { get; set; }
    }

    public class RelationshipSummaryAttribute
    {
        public const string PID = "pid";
        public const string PREV_PID = "previousPid";
        public const string SUB_ID = "subId";
        public const string PREV_SUB_ID = "previousSubId";

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public enum ServiceOutcome
    {
        TEST1
    }
}
