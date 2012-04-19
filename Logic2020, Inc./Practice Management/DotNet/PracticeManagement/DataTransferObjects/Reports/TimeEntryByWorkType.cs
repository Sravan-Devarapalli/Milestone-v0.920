using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using DataTransferObjects.TimeEntry;

namespace DataTransferObjects.Reports
{
    [DataContract]
    [Serializable]
    public class TimeEntryByWorkType
    {

        [DataMember]
        public TimeTypeRecord TimeType { get; set; }


        [DataMember]
        public string Note { get; set; }

        [DataMember]
        public double BillableHours { get; set; }

        [DataMember]
        public double NonBillableHours { get; set; }


        public double TotalHours
        {
            get
            {
                return BillableHours + NonBillableHours;
            }
        }

        public string NoteForExport
        {
            get 
            {
                return Note.Replace("\n", " ").Replace("\r"," ");
            }
        }

        public string HTMLNote
        {
            get
            {
                return Note.Replace("\n", "<br/>");
            }
        }



    }
}

