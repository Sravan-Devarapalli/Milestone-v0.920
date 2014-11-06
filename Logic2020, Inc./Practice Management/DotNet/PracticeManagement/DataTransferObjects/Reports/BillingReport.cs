using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects.Reports
{
    [DataContract]
    [Serializable]
    public class BillingReport
    {
        [DataMember]
        public Project Project
        {
            get;
            set;
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

        [DataMember]
        public PracticeManagementCurrency RangeProjected
        {
            get;
            set;
        }

        [DataMember]
        public PracticeManagementCurrency RangeActual
        {
            get;
            set;
        }

        [DataMember]
        public PracticeManagementCurrency SOWBudget
        {
            get;
            set;
        }

        [DataMember]
        public PracticeManagementCurrency ActualToDate
        {
            get;
            set;
        }

        [DataMember]
        public double ForecastedHours
        {
            get;
            set;
        }

        [DataMember]
        public double ActualHours
        {
            get;
            set;
        }

        [DataMember]
        public double ForecastedHoursInRange
        {
            get;
            set;
        }

        [DataMember]
        public double ActualHoursInRange
        {
            get;
            set;
        }

        public PracticeManagementCurrency Remaining
        {
            get
            {
                return SOWBudget - ActualToDate;
            }
        }

        public PracticeManagementCurrency DifferenceInCurrency
        {
            get
            {
                return RangeActual - RangeProjected;
            }
        }

        public double DifferenceInHours
        {
            get
            {
                return ActualHoursInRange - ForecastedHoursInRange;
            }
        }

        public double RemainingHours
        {
            get
            {
                return ForecastedHours - ActualHours;
            }
        }

        public string ProjectMangers
        {
            get
            { 
                StringBuilder managers=new StringBuilder();
                if(Project.ProjectManagers == null || Project.ProjectManagers.Count ==0)
                    return string.Empty;
                foreach (var item in Project.ProjectManagers)
                    managers.Append(item.HtmlEncodedName+"; ");
                return managers.ToString();
            }
        }

    }
}

