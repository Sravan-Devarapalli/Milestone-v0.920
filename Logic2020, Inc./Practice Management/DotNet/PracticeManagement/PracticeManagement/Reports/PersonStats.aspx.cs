using System;
using PraticeManagement.Controls;
using System.Web.Security;
using System.Web.UI.WebControls;
using DataTransferObjects;
using System.Threading;

namespace PraticeManagement.Reporting
{
    public partial class PersonStatsReport : PracticeManagementPageBase
    {
        #region "Constants"

        private const int MaxPeriodLength = 24;
        private const string CompanyPerformanceFilterKey = "CompanyPerformanceFilterKey";

        #endregion "Constants"

        #region "Properties"

        /// <summary>
        /// Gets a selected period start.
        /// </summary>
        private DateTime PeriodStart
        {
            get
            {
                return new DateTime(mpPeriodStart.SelectedYear, mpPeriodStart.SelectedMonth, Constants.Dates.FirstDay);
            }
        }

        /// <summary>
        /// Gets a selected period end.
        /// </summary>
        private DateTime PeriodEnd
        {
            get
            {
                return
                    new DateTime(mpPeriodEnd.SelectedYear, mpPeriodEnd.SelectedMonth,
                        DateTime.DaysInMonth(mpPeriodEnd.SelectedYear, mpPeriodEnd.SelectedMonth));
            }
        }       

        #endregion "Properties"

        protected void Page_Load(object sender, EventArgs e)
        {
            custPeriodLengthLimit.ErrorMessage = custPeriodLengthLimit.ToolTip =
                string.Format(custPeriodLengthLimit.ErrorMessage, MaxPeriodLength);
        }

        /// <summary>
        /// Adds to the performance grid one for each the month withing the selected period.
        /// </summary>
        protected override void Display()
        {
            if (!IsPostBack)
            {
                PopulateFiltersWithDefaultValues();
            }

            PersonStats[] personStats = CompanyPerformanceState.GetPersonStats(PeriodStart, PeriodEnd, Thread.CurrentPrincipal.Identity.Name,
                                                                               null, null, chbProjected.Checked, chbCompleted.Checked, chbActive.Checked, chbExperimental.Checked, chbInternal.Checked, chbInactive.Checked);

            repPersonStats.DisplayPersonStatsReport(personStats);
        }

        protected void custPeriod_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = GetPeriodLength() > 0;
        }

        protected void custPeriodLengthLimit_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = GetPeriodLength() <= MaxPeriodLength;
        }

        protected void btnUpdateView_Click(object sender, EventArgs e)
        {
            Page.Validate(valsPerformance.ValidationGroup);
            if (Page.IsValid)
            {
                Display();
            }
        }

        /// <summary>
        /// Executes preliminary operations to the view be ready to display the data.
        /// </summary>
        private void PopulateFiltersWithDefaultValues()
        {
            var filter = new CompanyPerformanceFilterSettings();
         
            // Set the default viewable interval.
            mpPeriodStart.SelectedYear = filter.StartYear;
            mpPeriodStart.SelectedMonth = filter.StartMonth;

            mpPeriodEnd.SelectedYear = filter.EndYear;
            mpPeriodEnd.SelectedMonth = filter.EndMonth;

            chbPeriodOnly.Checked = filter.TotalOnlySelectedDateWindow;

            chbActive.Checked = filter.ShowActive;
            chbCompleted.Checked = filter.ShowCompleted;
            chbExperimental.Checked = filter.ShowExperimental;
            chbProjected.Checked = filter.ShowProjected;
            chbInternal.Checked = filter.ShowInternal;
            chbInactive.Checked = filter.ShowInactive;
        }       

        /// <summary>
        /// Calculates a length of the selected period in the mounths.
        /// </summary>
        /// <returns>The number of the months within the selected period.</returns>
        private int GetPeriodLength()
        {
            int mounthsInPeriod =
                (mpPeriodEnd.SelectedYear - mpPeriodStart.SelectedYear) * Constants.Dates.LastMonth +
                (mpPeriodEnd.SelectedMonth - mpPeriodStart.SelectedMonth + 1);
            return mounthsInPeriod;
        }
    }
}

