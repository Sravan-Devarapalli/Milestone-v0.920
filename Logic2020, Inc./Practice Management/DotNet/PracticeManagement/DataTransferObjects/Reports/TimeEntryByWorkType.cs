using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using DataTransferObjects.TimeEntry;
using System.Web;

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

        [DataMember]
        public decimal? HourlyRate { get; set; }


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

        public string HtmlEncodedNoteForExport
        {
            get
            {
                return HttpUtility.HtmlEncode(NoteForExport);
            }
        }


        public string HTMLNote
        {
            get
            {
                return Note.Replace("\n", "<br/>");
            }
        }

        public string HtmlEncodedHTMLNote
        {
            get
            {
                return HttpUtility.HtmlEncode(HTMLNote).Replace("&lt;br/&gt;", "<br/>");
            }
        }
        


    }
}

