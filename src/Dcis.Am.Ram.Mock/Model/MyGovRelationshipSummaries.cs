using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dcis.Am.Ram.Mock.Model
{
    public class MyGovRelationshipSummaries
    {
        [JsonProperty("myGovId")]
        public string MyGovId { get; set; }

        [JsonProperty("httpStatusCode")]
        public string HttpStatusCode { get; set; }

        [JsonProperty("relationshipSummaries")]
        public List<RelationshipSummary> RelationshipSummaries { get; set; }
    }
}
