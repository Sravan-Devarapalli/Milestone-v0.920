﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace DataTransferObjects.Skills
{
    [DataContract]
    [Serializable]
    public class LookupBase
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public int? DisplayOrder { get; set; }

    }
}

