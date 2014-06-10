using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    [Serializable]
    [DataContract]
    public class ProjectFeedbackMail
    {
        [DataMember]
        public Project Project
        {
            get;
            set;
        }

        [DataMember]
        public List<ProjectFeedback> Resources
        {
            get;
            set;
        }

        [DataMember]
        public string ProjectManagersAliasList
        {
            get;
            set;
        }

        [DataMember]
        public string ClientDirectorAlias
        {
            get;
            set;
        }

        [DataMember]
        public string ProjectOwnerAlias
        {
            get;
            set;
        }

        [DataMember]
        public string SeniorManagerAlias
        {
            get;
            set;
        }
    }
}

