using Dcis.Am.Ram.Mock.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Dcis.Am.Ram.Mock.Controllers
{
    public class RamController : ControllerBase
    {
        public RamController(ILogger<RamController> logger)
        {
            Logger = logger;
        }

        public ILogger<RamController> Logger { get; }

        [SwaggerResponse((int)HttpStatusCode.OK, "", typeof(RelationshipSummary))]
        [SwaggerOperation("GetRelationshipSummaryById", "GET /api/v2/relationshipSummaries/{id}")]
        [Route("api/v2/relationshipSummaries/{id}")]
        [HttpGet]
        public IActionResult GetRelationshipSummaryById(string id,
                                                        [FromQuery(Name = "correlationId")]string correlationId,
                                                        [FromQuery(Name = "serviceOutcome")]string serviceOutcome,
                                                        [FromQuery(Name = "serviceProvider")]string serviceProvider)
        {
            Logger.LogTrace("GET /api/v2/relationshipSummaries/{id}");

            if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out Guid guid))
            {
                throw new KnownException("REB.DV.0009", "id must be a guid");
            }

            if ((string.IsNullOrWhiteSpace(correlationId) && !string.IsNullOrWhiteSpace(serviceOutcome)))
            {
                throw new KnownException("REC.CV.0008", "correlationid cannot be null");
            }

            if (!string.IsNullOrWhiteSpace(correlationId) && (string.IsNullOrWhiteSpace(serviceOutcome) || !Enum.TryParse(serviceOutcome.ToUpper(), true, out ServiceOutcome type)))
            {
                throw new KnownException("REC.CV.0009", "Invalid Serviceoutcome");
            }

            var summary = DataHelper.GetRelationshipSummary(guid, serviceProvider);

            return new JsonResult(summary);
        }

        [SwaggerResponse((int)HttpStatusCode.OK, "", typeof(List<RelationshipSummary>))]
        [SwaggerOperation("GetRelationshipSummariesByDelegateId", "GET /api/v2/relationshipSummaries/delegate/{providerCode}/{delegateId}/")]
        [Route("/api/v2/relationshipSummaries/delegate/{providerCode}/{delegateId}/")]
        [HttpGet]
        public IActionResult GetRelationshipSummariesByDelegateId(string providerCode,
                              string delegateId,
                              [FromQuery(Name = "filter")] RelationshipSummaryFilter filter,
                              [FromQuery(Name = "sort")] RelationshipSummarySort sort,
                              [FromQuery(Name = "fields")] string fields)
        {
            Logger.LogDebug("GET /api/v2/relationshipSummaries/delegate/{providerCode}/{delegateId}/");


            if (string.IsNullOrWhiteSpace(providerCode))
            {
                throw new KnownException("REB.DV.0008", "Provider code cannot be null");
            }
            if (string.IsNullOrWhiteSpace(delegateId))
            {
                throw new KnownException("REB.DV.0002", "Delegate party cannot be null");
            }

            if (!(providerCode.Equals(IdentityProvider.MYGOVID, StringComparison.OrdinalIgnoreCase)
                || providerCode.Equals(IdentityProvider.EXCHANGEID, StringComparison.OrdinalIgnoreCase)))
            {
                throw new KnownException("REC.DV.0014", "Invalid Provider Code");
            }

            string subjectIdType = filter?.SubjectIdType;
            string relationshipType = filter?.RelationshipType;
            string text = filter?.Text;
            string serviceProvider = filter?.ServiceProvider;
            DateTime? lastUpdatedFromDate = null;
            string d = filter?.LastUpdatedFromDate;
            if (d != null)
            {
                d = d.Replace(" ", "+"); // This is a hack to put the plus sign back after url decode       
                lastUpdatedFromDate = DateTime.Parse(d);
            }

            string sortAbn = sort?.Abn;
            string sortBusinessName = sort?.BusinessName;
            string sortLastUpdatedDate = sort?.LastUpdatedDate;

            var requestHeaders = new Microsoft.AspNetCore.Http.Headers.RequestHeaders(Request.Headers);
            var rangeHeader = requestHeaders.Range?.Ranges.SingleOrDefault();

            int itemFrom = 0;
            int pageSize = 99;

            if (rangeHeader != null)
            {
                itemFrom = Convert.ToInt32(rangeHeader.From.GetValueOrDefault(0));
                pageSize = Convert.ToInt32(rangeHeader.To.GetValueOrDefault(99));
            }

            // Not supported
            if (!string.IsNullOrEmpty(fields))
            {
                throw new KnownException("REC.CV.0005", "Invalid fields value provided");
            }

            string code = providerCode.ToUpper();

            var list = DataHelper.GetRelationshipSummariesByDelegateId(getExecutingParty(), delegateId, code, subjectIdType, text, itemFrom, pageSize, out string statusCode, out int total);

            JsonResult jsonResult = new JsonResult(list);
            jsonResult.StatusCode = int.Parse(statusCode);
            int returnedItemTo = list.Any() ? (itemFrom + list.Count - 1) : 0;
            Response.Headers.Add("Content-Range", string.Format("items {0}-{1}/{2}", itemFrom, returnedItemTo, total));
            return jsonResult;
        }

        private string getExecutingParty() => string.Empty;

    }
}