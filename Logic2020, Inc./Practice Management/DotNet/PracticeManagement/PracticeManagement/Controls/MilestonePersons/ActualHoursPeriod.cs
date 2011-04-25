using System;
using System.Collections.Generic;
using System.Linq;
using DataTransferObjects;

namespace PraticeManagement.Controls.MilestonePersons
{
    public class ActualHoursPeriod : DatePeriodBase
    {
        public ActualHoursPeriod(IActivityPeriod activityPeriod)
            : base(activityPeriod)
        {
        }

        public double ActualToDate
        {
            get { return ActivityPeriod.ActualActivity.Sum(te => te.ActualHours); }
        }

        public double ForecastedToDate
        {
            get { return ActivityPeriod.ActualActivity.Sum(te => te.ForecastedHours); }
        }

        public override IEnumerable<DatePoint> Period
        {
            get
            {
                var tes = ActivityPeriod.ActualActivity;
                if (tes.Count > 0)
                {
                    var points = new List<DatePoint>(ActivityPeriod.EmptyPeriodPoints);

                    foreach (var te in tes)
                    {
                        var entryRecord = te;
                        var index = points.FindIndex(obj => obj.Date == entryRecord.MilestoneDate);

                        if (index < 0)
                        {
                            //throw new Exception(
                            //    string.Format(
                            //         Resources.Messages.UnableToFindDateIndex,
                            //         entryRecord.MilestoneDate.ToString(Constants.Formatting.EntryDateFormat),
                            //         ActivityPeriod.StartDate.ToString(Constants.Formatting.EntryDateFormat),
                            //         ActivityPeriod.EndDate.ToString(Constants.Formatting.EntryDateFormat)));
                            index = 0;
                            points.Insert(index, DatePoint.FromTimeEntry(entryRecord));
                        }

                        if (points[index].Value.HasValue)
                            points[index].Value += te.ActualHours;
                        else
                            points[index].Value = te.ActualHours;
                    }

                    points.Sort();

                    return points;
                }

                return new List<DatePoint>();
            }
        }
    }
}
