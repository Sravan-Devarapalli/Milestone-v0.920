using System;

namespace DataTransferObjects.ContextObjects
{
    public class ConsultantMilestonesContext
    {
        public int PersonId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IncludeActiveProjects { get; set; }
        public bool IncludeProjectedProjects { get; set; }
        public bool IncludeInactiveProjects { get; set; }
        public bool IncludeCompletedProjects { get; set; }
        public bool IncludeInternalProjects { get; set; }
        public bool IncludeExperimentalProjects { get; set; }
        public bool IncludeDefaultMileStone { get; set; }
    }
}

