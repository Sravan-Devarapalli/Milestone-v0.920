using System.Collections.Generic;
using System.Linq;
using DataTransferObjects;

namespace PraticeManagement.Controls.MilestonePersons
{
    public class ProjectedHoursPeriod : DatePeriodBase
    {
        public ProjectedHoursPeriod(IActivityPeriod activityPeriod)
            : base(activityPeriod)
        {
        }

        public override IEnumerable<DatePoint> Period
        {
            get { return ActivityPeriod.ProjectedActivity; }
        }

        public double TotalProjectedHours
        {
            get
            {
                return Period.Sum(point => point.Value.HasValue && !point.DayOff ? point.Value.Value : 0.0);
            }
        }
    }
}

