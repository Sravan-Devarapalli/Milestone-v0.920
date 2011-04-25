using System;
using System.Collections.Generic;
using DataTransferObjects;

namespace PraticeManagement.Controls.MilestonePersons
{
    public abstract class DatePeriodBase
    {
        #region Protected Properties

        protected IActivityPeriod ActivityPeriod { get; set; }

        #endregion  

        #region Constructor

        /// <summary>
        /// Init constructor of ProjectedHoursPeriod.
        /// </summary>
        protected DatePeriodBase(IActivityPeriod activityPeriod)
        {
            ActivityPeriod = activityPeriod;
        }

        #endregion

        #region Methods

        public abstract IEnumerable<DatePoint> Period { get; }

        public virtual IEnumerable<DatePoint> CumulativePeriod
        {
            get
            {
                var ptValue = 0.0;

                foreach (var pt in Period)
                {
                    ptValue += pt.Value.HasValue ? pt.Value.Value : 0.0;

                    yield return new DatePoint
                                     {
                                         Date = pt.Date,
                                         Value = ptValue
                                     };
                }
            }
        }

        #endregion

        #region Date routines

        protected int DateToCoord(DateTime date)
        {
            return date.Subtract(ActivityPeriod.StartDate).Days;
        }

        #endregion
    }
}

