using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Web;

namespace DataTransferObjects
{
    [DataContract]
    [Serializable]
    public class Attribution
    {
        [DataMember]
        public int? Id
        {
            get;
            set;
        }

        [DataMember]
        public Person Person
        {
            get;
            set;
        }

        [DataMember]
        public Practice Practice
        {
            get;
            set;
        }

        [DataMember]
        public int TargetId
        {
            get;
            set;
        }

        [DataMember]
        public Title Title
        {
            get;
            set;
        }

        [DataMember]
        public string TargetName
        {
            get;
            set;
        }

        [DataMember]
        public DateTime? StartDate
        {
            get;
            set;
        }

        [DataMember]
        public DateTime? EndDate
        {
            get;
            set;
        }

        [DataMember]
        public decimal CommissionPercentage
        {
            get;
            set;
        }

        [DataMember]
        public AttributionTypes AttributionType
        {
            get;
            set;
        }

        [DataMember]
        public AttributionRecordTypes AttributionRecordType
        {
            get;
            set;
        }

        public string HtmlEncodedTargetName
        {
            get { return HttpUtility.HtmlEncode(TargetName);
            }
            set { TargetName = HttpUtility.HtmlDecode(value); }
        }

        public bool IsEditMode { get; set; }

        public bool IsNewEntry { get; set; }

        public bool IsCheckBoxChecked { get; set; }

    }
}

