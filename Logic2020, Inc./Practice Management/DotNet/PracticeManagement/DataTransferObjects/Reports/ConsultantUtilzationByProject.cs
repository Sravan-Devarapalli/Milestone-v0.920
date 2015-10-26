using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects.Reports
{
    [DataContract]
    [Serializable]
    public class ConsultantUtilzationByProject
    {
        [DataMember]
        public List<UtilizationByProject> UtilizationProject
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
        public decimal AvailableHours
        {
            get;
            set;
        }

        public string Format
        {
            get
            {
                var stringBuilder = new StringBuilder();
                if (UtilizationProject != null)
                {
                    foreach (var project in UtilizationProject)
                    {
                        if (project.Utilization == -1)
                            return "-1";
                        else if (project.Project.Id.HasValue)
                            stringBuilder.Append(project.Project.ProjectNumber + " - " + project.Project.Name + " - " + (AvailableHours != 0 ? Math.Round((100) * project.ProjectedHours / AvailableHours, 2) : 0) + "%\n");
                        else
                            return "0";
                    }
                    return stringBuilder.ToString();
                }
                return "0";
            }
        }
    }
}

