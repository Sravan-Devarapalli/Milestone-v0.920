using System;
using System.Runtime.Serialization;

namespace DataTransferObjects.ContextObjects
{
    [DataContract]
    [Serializable]
    public class EmailContext
    {
        [DataMember]
        public string StorerProcedureName
        {
            get;
            set;
        }

        [DataMember]
        public int EmailTemplateId
        {
            get;
            set;
        }
    }
}

