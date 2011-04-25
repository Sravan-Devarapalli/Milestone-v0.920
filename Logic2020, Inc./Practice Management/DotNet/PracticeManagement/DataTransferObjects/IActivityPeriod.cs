using System;
using System.Collections.Generic;
using DataTransferObjects.TimeEntry;

namespace DataTransferObjects
{
    /// <summary>
    /// Represents date period and corresponding methods and properties
    /// </summary>
    public interface IActivityPeriod
    {
        /// <summary>
        /// Start date
        /// </summary>
        DateTime StartDate { get; set; }

        /// <summary>
        /// End date
        /// </summary>
        DateTime EndDate { get; set; }

        /// <summary>
        /// Empty DatePoint for each date of the period
        /// </summary>
        List<DatePoint> EmptyPeriodPoints { get; set; }

        /// <summary>
        /// Projected values for each date of the period
        /// </summary>
        List<DatePoint> ProjectedActivity { get; set; }

        /// <summary>
        /// Actual hours for each date of the period
        /// </summary>
        List<TimeEntryRecord> ActualActivity { get; set; }
    }
}

