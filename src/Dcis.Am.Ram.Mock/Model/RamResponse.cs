using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dcis.Am.Ram.Mock.Model
{
    public class RamResponse
    {
        [JsonProperty("ramResponses")]
        public List<MyGovRelationshipSummaries> RamResponses { get; set; }
    }
}
