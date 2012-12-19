using System;
using System.Runtime.Serialization;
using System.Web;

namespace DataTransferObjects
{
    [Serializable]
    [DataContract]
    public class Title
    {
        [DataMember]
        public int TitleId
        {
            get;
            set;
        }

        [DataMember]
        public string TitleName
        {
            get;
            set;
        }

        [DataMember]
        public TitleType TitleType
        {
            get;
            set;
        }

        [DataMember]
        public int SortOrder
        {
            get;
            set;
        }

        [DataMember]
        public int PTOAccrual
        {
            get;
            set;
        }

        [DataMember]
        public int? MinimumSalary
        {
            get;
            set;
        }

        [DataMember]
        public int? MaximumSalary
        {
            get;
            set;
        }

        [DataMember]
        public bool TitleInUse
        {
            get;
            set;
        }

        public string HtmlEncodedTitleName
        {
            get
            {
                return HttpUtility.HtmlEncode(TitleName);
            }
        }
    }
}

