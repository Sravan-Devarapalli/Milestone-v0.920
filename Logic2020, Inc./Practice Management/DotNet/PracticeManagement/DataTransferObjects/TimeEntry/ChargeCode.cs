using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects.TimeEntry
{
    [DataContract]
    [Serializable]
    public class ChargeCode
    {
        private const string seperator = " - ";


        public string ChargeCodeName
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(Client.Name);
                sb.Append(seperator);
                sb.Append(ProjectGroup.Name);
                sb.Append(seperator);
                sb.Append(Project.ProjectNumber);
                sb.Append(seperator);
                sb.Append(Project.Name);
                sb.Append(seperator);
                sb.Append(Phase);
                sb.Append(seperator);
                sb.Append(TimeType.Name);

                return sb.ToString();

            }

        }

        [DataMember]
        public int ChargeCodeId
        {
            get;
            set;
        }

        [DataMember]
        public Project Project
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
        public ProjectGroup ProjectGroup
        {
            get;
            set;

        }

        [DataMember]
        public int Phase
        {
            get;
            set;
        }

        [DataMember]
        public TimeTypeRecord TimeType
        {
            get;
            set;

        }
    }
}

