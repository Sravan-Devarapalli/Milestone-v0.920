using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects.Reports
{
    [DataContract]
    [Serializable]
    public class GroupByPersonAuditReport
    {
     


        [DataMember]
        public List<TimeEntryAudit> TimeEntries
        {
            get;
            set;
        }


    }
}

