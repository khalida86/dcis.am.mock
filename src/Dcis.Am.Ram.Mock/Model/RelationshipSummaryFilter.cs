using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dcis.Am.Ram.Mock.Model
{
    public class RelationshipSummaryFilter
    {
        public string SubjectIdType { get; internal set; }
        public string RelationshipType { get; internal set; }
        public string Text { get; internal set; }
        public string ServiceProvider { get; internal set; }
        public string LastUpdatedFromDate { get; internal set; }
    }
}
