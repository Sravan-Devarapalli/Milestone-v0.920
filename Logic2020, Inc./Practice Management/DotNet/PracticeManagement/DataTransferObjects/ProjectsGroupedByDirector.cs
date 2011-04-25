using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    [DataContract]
    [Serializable]
    public class ProjectsGroupedByPerson
    {
        [DataMember]
        public int PersonId
        {
            get;
            set;
        }

        [DataMember]
        public string FirstName
        {
            get;
            set;
        }

        [DataMember]
        public string LastName
        {
            get;
            set;
        }

        [DataMember]
        public DateTime HireDate
        {
            get;
            set;
        }

        [DataMember]
        public DateTime? TerminationDate
        {
            get;
            set;
        }

        [DataMember]
        public Dictionary<DateTime, ComputedFinancials> ProjectedFinancialsByMonth
        {
            get;
            set;
        }

        [DataMember]
        public List<ProjectsGroupedByClient> GroupedClients
        {
            get;
            set;
        }
        
        [DataMember]
        public ComputedFinancials ComputedFinancials
        {
            get;
            set;
        }
    }

    [DataContract]
    [Serializable]
    public class ProjectsGroupedByClient : Client
    {
        [DataMember]
        public List<ProjectsGroupedByClientGroup> GroupedClientGroups
        {
            get;
            set;
        }

        [DataMember]
        public ComputedFinancials ComputedFinancials
        {
            get;
            set;
        }

        [DataMember]
        public Dictionary<DateTime, ComputedFinancials> ProjectedFinancialsByMonth
        {
            get;
            set;
        }

        [DataMember]
        public List<Project> ProjectsWithoutClientGroup
        {
            get;
            set;
        }

        [DataMember]
        public List<MilestonePerson> MilestonePersonsWithoutClientGroup
        {
            get;
            set;
        }
    }

    [DataContract]
    [Serializable]
    public class ProjectsGroupedByClientGroup : ProjectGroup
    {
        [DataMember]
        public ComputedFinancials ComputedFinancials
        {
            get;
            set;
        }

        [DataMember]
        public Dictionary<DateTime, ComputedFinancials> ProjectedFinancialsByMonth
        {
            get;
            set;
        }

        [DataMember]
        public List<Project> Projects
        {
            get;
            set;
        }

        [DataMember]
        public List<MilestonePerson> MilestonePersons
        {
            get;
            set;
        }

    }

    [DataContract]
    [Serializable]
    public class ProjectsGroupedByPractice
    {
        [DataMember]
        public int PracticeId
        {
            get;
            set;
        }

        [DataMember]
        public string Name
        {
            get;
            set;
        }

        [DataMember]
        public Person PracticeManager
        {
            get;
            set;
        }
        
        [DataMember]
        public List<PracticeManagerHistory> PreviousPracticeManagers
        {
            get;
            set;
        }

        [DataMember]
        public Dictionary<DateTime, ComputedFinancials> ProjectedFinancialsByMonth
        {
            get;
            set;
        }

        [DataMember]
        public List<ProjectsGroupedByClient> GroupedClients
        {
            get;
            set;
        }

        [DataMember]
        public ComputedFinancials ComputedFinancials
        {
            get;
            set;
        }
    }
}

