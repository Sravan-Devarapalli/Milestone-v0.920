using System;
using System.Runtime.Serialization;
using System.Text;

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

                if (Client != null)
                {
                    sb.Append(Client.HtmlEncodedName);
                }

                sb.Append(seperator);

                if (ProjectGroup != null)
                {
                    sb.Append(ProjectGroup.HtmlEncodedName);
                }
                sb.Append(seperator);

                if (Project != null)
                {
                    sb.Append(Project.ProjectNumber);
                }
                sb.Append(seperator);

                if (Project != null)
                {
                    sb.Append(Project.HtmlEncodedName);
                }

                sb.Append(seperator);

                if (Phase != 0)
                {
                    sb.Append("Phase" + Phase);
                    sb.Append(seperator);
                }

                if (TimeType != null)
                {
                    sb.Append(TimeType.Name);
                }

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
        public TimeEntrySectionType TimeEntrySection
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
