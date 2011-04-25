using System;

namespace PraticeManagement.Controls.TimeEntry
{
    public class WeekChangedEventArgs : EventArgs
    {
        private DateTime startDate;
        private DateTime endDate;

        /// <summary>
        /// Init constructor of WeekChangedEventArguments.
        /// </summary>
        public WeekChangedEventArgs(System.DateTime startDate, System.DateTime endDate)
        {
            this.startDate = startDate;
            this.endDate = endDate;
        }


        #region Properties
        public System.DateTime StartDate
        {
            get
            {
                return this.startDate;
            }
        }

        public System.DateTime EndDate
        {
            get
            {
                return this.endDate;
            }
        }
        #endregion
    }
}

