using Dcis.Am.Ram.Mock.Model;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Dcis.Am.Ram.Mock.Controllers
{
    // We are not using Factory class as we are directly loading data from AM's RAMMockConfig.json file.

    //[ApiController]
    public class FactoryController
    {
        //[Route("api/data")]
        //[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(List<RelationshipSummary>))]
        //[SwaggerOperation("Generate Data", "Use this api to generate random data. Once data is generated copy the output manually to RamMockConfig.json file")]
        //[HttpGet]
        public IActionResult GenerateData()
        {
            var relationSummaries = new List<RelationshipSummary>();

            for (int i = 0; i < 20; i++)
            {
                Guid guid = Guid.NewGuid();
                for (int random = 0; random < new Random().Next(1, 5); random++)
                {
                    relationSummaries.Add(GetFakeRelationshipSummary(guid, i + 1));
                }
            }

            return new JsonResult(relationSummaries);
        }

        private static RelationshipSummary GetFakeRelationshipSummary(Guid guid, int counter)
        {
            return new RelationshipSummary
            {
                Id = guid.ToString(),
                SubjectId = $"A{counter} Subject",
                SubjectIdType = $"A{counter} SubjectType",
                SubjectName = $"A{counter} SubjectName",
                RelationshipType = $"A{counter} RelationshipType",
                StartTimestamp = System.DateTime.Now.AddDays(-7),
                EndTimestamp = System.DateTime.Now,
                Permissions = new List<string> { "Permission1", "Permission2", "Permission3" },
                Roles = new List<string> { "Role1", "Role2", "Role3", "Role4" },
                Email = $"A{counter}@test.com",
                LastModified = System.DateTime.Now,
                Attributes = new List<RelationshipSummaryAttribute>
                 {
                     new RelationshipSummaryAttribute
                     {
                          Name = $"A{counter}.attribute1",
                          Value = $"A{counter}.value1"
                     },
                     new RelationshipSummaryAttribute
                     {
                          Name = $"A{counter}.attribute2",
                          Value = $"A{counter}.value2"
                     },
                     new RelationshipSummaryAttribute
                     {
                          Name = $"A{counter}.attribute3",
                          Value = $"A{counter}.value3"
                     },
                 }
            };
        }
    }
}
