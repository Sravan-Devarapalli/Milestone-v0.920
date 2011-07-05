using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    [DataContract]
    [Serializable]
    public class ProjectAttachment
    {
        [DataMember]
        public String AttachmentFileName
        {
            get;
            set;
        }

        [DataMember]
        public Byte[] AttachmentData
        {
            get;
            set;
        }

        [DataMember]
        public int AttachmentSize
        {
            get;
            set;
        }

        [DataMember]
        public DateTime? UploadedDate
        {
            get;
            set;
        }

    }
}

