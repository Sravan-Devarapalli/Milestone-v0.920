﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace DataTransferObjects
{
    [DataContract]
    [Serializable]
    public class ConsultantDemandItem
    {
        [DataMember]
        public Person Consultant
        {
            set;
            get;
        }


        [DataMember]
        public string QuantityString
        {
            get;
            set;
        }

        [DataMember]
        public Client Client
        {
            get;
            set;
        }

        [DataMember]
        public int ObjectId
        {
            get;
            set;
        }

        [DataMember]
        public string ObjectName
        {
            get;
            set;
        }

        [DataMember]
        public string ObjectNumber
        {
            get;
            set;
        }

        [DataMember]
        public int ObjectType
        {
            set;
            get;
        }

        [DataMember]
        public int ObjectStatusId
        {
            set;
            get;
        }

        [DataMember]
        public DateTime StartDate
        {
            get;
            set;
        }

        [DataMember]
        public DateTime EndDate
        {
            get;
            set;
        }
    }
}

