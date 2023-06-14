using Dcis.Am.Ram.Mock.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dcis.Am.Ram.Mock
{
    public static class DataHelper
    {
        static RamResponse RamResponse { get; set; }

        public static void LoadRelationshipSummaries()
        {
            var mockData = File.ReadAllText(".\\RamMockConfig.json");
            RamResponse = JsonConvert.DeserializeObject<RamResponse>(mockData);
        }

        public static RelationshipSummary GetRelationshipSummary(Guid guid, string serviceProvider)
        {
            var relationshipSummaries = RamResponse.RamResponses.Where(x => x.MyGovId == guid.ToString()).FirstOrDefault()?.RelationshipSummaries;
            return relationshipSummaries.FirstOrDefault();
        }

        public static List<RelationshipSummary> GetRelationshipSummariesByDelegateId(string executingParty, string delegateId, string code, string subjectIdType, string text, int rangeFrom, int rangeTo, out string statusCode, out int total)
        {
            var relationshipSummaries = RamResponse.RamResponses.Where(x => x.MyGovId == delegateId).FirstOrDefault()?.RelationshipSummaries.Where(x => x.SubjectIdType == "ABN").OrderBy(x => x.LastModified).ToList();
            total = relationshipSummaries.Count;
            if (rangeTo >= total)
                rangeTo = total - 1;
            relationshipSummaries = relationshipSummaries?.GetRange(rangeFrom, rangeTo - rangeFrom + 1);
            statusCode = RamResponse.RamResponses.Where(x => x.MyGovId == delegateId).FirstOrDefault()?.HttpStatusCode;
            return relationshipSummaries;
        }
    }
}
