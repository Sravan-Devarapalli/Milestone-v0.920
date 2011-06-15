using System;
using System.Diagnostics;
using DataTransferObjects;

namespace PraticeManagement.Controls
{
    /// <summary>
    /// 	Represents a filter settings for the Company Performance page.
    /// </summary>
    [Serializable]
    public class CompanyPerformanceFilterSettings
    {
        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int? clientIdValue;

        private string clientIdsList;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int? endDayValue;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int endMonthValue;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int endYearValue;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool hideAdvancedFilterValue;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int? practiceIdValue;
        private string practiceIdsList;
        private bool excludeInternalPractices;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int? projectOwnerIdValue;
        private string projectOwnerIdsList;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int? projectGroupIdValue;
        private string projectGroupIdsList;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int? salespersonIdValue;
        private string salespersonIdsList;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool showActiveValue;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool showCompletedValue;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool showInternalValue;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool showInactiveValue;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool showExperimentalValue;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool showProjectedValue;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int? startDayValue;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int startMonthValue;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int startYearValue;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool isGroupByPersonPage;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int periodSelectedValue;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int viewSelectedValue;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ProjectCalculateRangeType calculateRangeSelectedValue;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool totalOnlySelectedDateWindowValue;

        #endregion

        #region Properties

        /// <summary>
        /// 	Gets or sets a period.
        /// </summary>
        public int PeriodSelected
        {
            get { return periodSelectedValue; }
            set { periodSelectedValue = value; }
        }


        /// <summary>
        /// 	Gets or sets a value for the results view (10/25/50/ALL).
        /// </summary>
        public int ViewSelected
        {
            get { return viewSelectedValue; }
            set { viewSelectedValue = value; }
        }

        /// <summary>
        /// 	Gets or sets a period range for GrandTotal calculation.
        /// </summary>
        public ProjectCalculateRangeType CalculateRangeSelected
        {
            get { return calculateRangeSelectedValue; }
            set { calculateRangeSelectedValue = value; }
        }

        /// <summary>
        /// 	Gets or sets a start year.
        /// </summary>
        public int StartYear
        {
            get { return startYearValue; }
            set { startYearValue = value; }
        }

        /// <summary>
        /// 	Gets or sets a start month.
        /// </summary>
        public int StartMonth
        {
            get { return startMonthValue; }
            set { startMonthValue = value; }
        }


        /// <summary>
        /// 	Gets or sets a start Day.
        /// </summary>
        public int? StartDay
        {
            get { return startDayValue; }
            set { startDayValue = value; }
        }

        /// <summary>
        /// 	Gets or sets an end year.
        /// </summary>
        public int EndYear
        {
            get { return endYearValue; }
            set { endYearValue = value; }
        }

        /// <summary>
        /// 	Gets or sets an end month.
        /// </summary>
        public int EndMonth
        {
            get { return endMonthValue; }
            set { endMonthValue = value; }
        }

        /// <summary>
        /// 	Gets or sets a end Day.
        /// </summary>
        public int? EndDay
        {
            get { return endDayValue; }
            set { endDayValue = value; }
        }
        /// <summary>
        /// 	Gets or sets whether the Active projects are shown.
        /// </summary>
        public bool ShowActive
        {
            get { return showActiveValue; }
            set { showActiveValue = value; }
        }

        /// <summary>
        /// 	Gets or sets whether the Internal projects are shown.
        /// </summary>
        public bool ShowInternal
        {
            get { return showInternalValue; }
            set { showInternalValue = value; }
        }

        /// <summary>
        /// 	Gets or sets whether the Inactive projects are shown.
        /// </summary>
        public bool ShowInactive
        {
            get { return showInactiveValue; }
            set { showInactiveValue = value; }
        }

        /// <summary>
        /// 	Gets or sets whether the Projected projects are shown.
        /// </summary>
        public bool ShowProjected
        {
            get { return showProjectedValue; }
            set { showProjectedValue = value; }
        }

        /// <summary>
        /// 	Gets or sets whether the Completed projects are shown.
        /// </summary>
        public bool ShowCompleted
        {
            get { return showCompletedValue; }
            set { showCompletedValue = value; }
        }

        /// <summary>
        /// 	Gets or sets whether the Experimental projects are shown.
        /// </summary>
        public bool ShowExperimental
        {
            get { return showExperimentalValue; }
            set { showExperimentalValue = value; }
        }

        /// <summary>
        /// 	Gets or sets a Client filter.
        /// </summary>
        public int? ClientId
        {
            get { return clientIdValue; }
            set { clientIdValue = value; }
        }

        /// <summary>
        /// 	Gets or sets a Client filter.
        /// </summary>
        public int? ProjectGroupId
        {
            get { return projectGroupIdValue; }
            set { projectGroupIdValue = value; }
        }

        /// <summary>
        /// 	Gets or sets a Salesperson filter.
        /// </summary>
        public int? SalespersonId
        {
            get { return salespersonIdValue; }
            set { salespersonIdValue = value; }
        }

        /// <summary>
        /// 	Gets or sets a Practice Manager filter.
        /// </summary>
        public int? ProjectOwnerId
        {
            get { return projectOwnerIdValue; }
            set { projectOwnerIdValue = value; }
        }

        /// <summary>
        /// 	Gets or sets a Practice filter.
        /// </summary>
        public int? PracticeId
        {
            get { return practiceIdValue; }
            set { practiceIdValue = value; }
        }

        /// <summary>
        /// 	Gets and sets Practice ids list
        /// </summary>
        public string PracticeIdsList
        {
            get { return practiceIdsList; }

            set { practiceIdsList = value; }
        }

        /// <summary>
        /// Gets and sets a flagindicates whether or not to include internal Practices
        /// </summary>
        public bool ExcludeInternalPractices
        {
            get { return excludeInternalPractices; }

            set { excludeInternalPractices = value; }
        }

        /// <summary>
        /// 	Gets and sets Project Group ids list
        /// </summary>
        public string ProjectGroupIdsList
        {
            get { return projectGroupIdsList; }

            set { projectGroupIdsList = value; }
        }

        /// <summary>
        /// 	Gets and sets Practice Manager ids list
        /// </summary>
        public string ProjectOwnerIdsList
        {
            get { return projectOwnerIdsList; }

            set { projectOwnerIdsList = value; }
        }

        /// <summary>
        /// 	Gets and sets Salesperson ids list
        /// </summary>
        public string SalespersonIdsList
        {
            get { return salespersonIdsList; }

            set { salespersonIdsList = value; }
        }

        /// <summary>
        /// 	Gets and sets client ids list
        /// </summary>
        public string ClientIdsList
        {
            get { return clientIdsList; }

            set { clientIdsList = value; }
        }

        /// <summary>
        /// 	Gets or sets whether totals are shown only in selected date window.
        /// </summary>
        public bool TotalOnlySelectedDateWindow
        {
            get { return totalOnlySelectedDateWindowValue; }
            set { totalOnlySelectedDateWindowValue = value; }
        }

        /// <summary>
        /// 	Gets or sets whether the advanced filter is hidded.
        /// </summary>
        public bool HideAdvancedFilter
        {
            get { return hideAdvancedFilterValue; }
            set { hideAdvancedFilterValue = value; }
        }

        /// <summary>
        /// 	Gets a date window start.
        /// </summary>
        public DateTime PeriodStart
        {
            get
            {
                return new DateTime(StartYear, StartMonth,
                    StartDay.HasValue ? StartDay.Value : Constants.Dates.FirstDay);
            }
        }

        /// <summary>
        /// 	Gets a date window end.
        /// </summary>
        public DateTime PeriodEnd
        {
            get
            {
                return new DateTime(EndYear, EndMonth,
                    EndDay.HasValue ? EndDay.Value : DateTime.DaysInMonth(EndYear, EndMonth));
            }
        }

        /// <summary>
        /// indicates if the projects are required for group by director/Practice manager page
        /// so that it will be used to determine which method to use for getting results/
        /// </summary>
        public bool IsGroupByPersonPage
        {
            get { return isGroupByPersonPage; }
            set { isGroupByPersonPage = value; }
        }

        #endregion

        #region Construction

        /// <summary>
        /// 	Creates a new instance of the <see cref = "CompanyPerformanceFilterSettings" /> class.
        /// </summary>
        public CompanyPerformanceFilterSettings()
        {
            var thisMonth = DateTime.Today;
            thisMonth = new DateTime(thisMonth.Year, thisMonth.Month, Constants.Dates.FirstDay);

            // Set the default viewable interval.
            StartYear = thisMonth.Year;
            StartMonth = thisMonth.Month;


            var periodEnd = thisMonth.AddMonths(Constants.Dates.DefaultViewableMonths);
            EndYear = periodEnd.Year;
            EndMonth = periodEnd.Month;

            PeriodSelected = 3; //Here 3 represents Last 3 months.
            ViewSelected = 10; //Here 10 represents View 10 results.
            CalculateRangeSelected = ProjectCalculateRangeType.ProjectValueInRange;

            // Project status filter defaults
            ShowActive = true;
            ShowCompleted = true;
            ShowProjected = true;
            ShowInternal = true;

            HideAdvancedFilter = true;
            ExcludeInternalPractices = false;
            ClientIdsList = null;
            ProjectOwnerIdsList = null;
            PracticeIdsList = null;
            ProjectGroupIdsList = null;
            SalespersonIdsList = null;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 	Determines whether objects are equal
        /// </summary>
        /// <param name = "obj">An object to be compared.</param>
        /// <returns>true if the specified instance is equals to the current one and false otherwise.</returns>
        public override bool Equals(object obj)
        {
            var compareObj = obj as CompanyPerformanceFilterSettings;
            bool result;
            if (compareObj == null)
            {
                // the specified object is not an instance of the CompanyPerformanceFilterSettings class
                result = false;
            }
            else
            {
                // Comparing all significant properties
                result =
                    // date window
                    StartMonth == compareObj.StartMonth &&
                    StartYear == compareObj.StartYear &&
                    EndMonth == compareObj.EndMonth &&
                    EndYear == compareObj.EndYear &&
                    PeriodSelected == compareObj.PeriodSelected &&
                    ((EndDay.HasValue && compareObj.EndDay.HasValue && EndDay.Value == compareObj.EndDay.Value)
                    || (!EndDay.HasValue && !compareObj.EndDay.HasValue)) &&
                     ((StartDay.HasValue && compareObj.StartDay.HasValue && StartDay.Value == compareObj.StartDay.Value)
                    || (!StartDay.HasValue && !compareObj.StartDay.HasValue)) &&
                    // filters
                    ClientId == compareObj.ClientId &&
                    ProjectGroupId == compareObj.ProjectGroupId &&
                    ProjectOwnerId == compareObj.ProjectOwnerId &&
                    PracticeId == compareObj.PracticeId &&
                    SalespersonId == compareObj.SalespersonId &&
                    // project status
                    ShowActive == compareObj.ShowActive &&
                    ShowCompleted == compareObj.ShowCompleted &&
                    ShowExperimental == compareObj.ShowExperimental &&
                    ShowProjected == compareObj.ShowProjected &&
                    ShowInternal == compareObj.ShowInternal &&
                    ShowInactive == compareObj.ShowInactive &&
                    // total range
                    TotalOnlySelectedDateWindow == compareObj.TotalOnlySelectedDateWindow &&
                    CalculateRangeSelected == compareObj.CalculateRangeSelected &&
                    ExcludeInternalPractices == compareObj.ExcludeInternalPractices &&

                    ClientIdsList == compareObj.ClientIdsList &&
                    SalespersonIdsList == compareObj.SalespersonIdsList &&
                    ProjectOwnerIdsList == compareObj.ProjectOwnerIdsList &&
                    PracticeIdsList == compareObj.PracticeIdsList &&
                    ProjectGroupIdsList == compareObj.ProjectGroupIdsList &&


                    IsGroupByPersonPage == compareObj.IsGroupByPersonPage;
            }

            return result;
        }

        /// <summary>
        /// 	Serves as a hash function for the <see cref = "CompanyPerformanceFilterSettings" /> class.
        /// </summary>
        /// <returns>A computed hash.</returns>
        public override int GetHashCode()
        {
            // Calculate a sum of all properties.
            return Convert.ToInt32(StartMonth) +
                   Convert.ToInt32(StartYear) +
                   Convert.ToInt32(EndMonth) +
                   Convert.ToInt32(EndYear) +
                   Convert.ToInt32(PeriodSelected) +
                // filters
                   Convert.ToInt32(ClientId) +
                   Convert.ToInt32(ProjectGroupId) +
                   Convert.ToInt32(ProjectOwnerId) +
                   Convert.ToInt32(PracticeId) +
                   Convert.ToInt32(SalespersonId) +
                // project status
                   Convert.ToInt32(ShowActive) +
                   Convert.ToInt32(ShowCompleted) +
                   Convert.ToInt32(ShowExperimental) +
                   Convert.ToInt32(ShowProjected) +
                   Convert.ToInt32(ShowInternal) +
                   Convert.ToInt32(ShowInactive) +
                // total range
                   Convert.ToInt32(CalculateRangeSelected) +
                   Convert.ToInt32(TotalOnlySelectedDateWindow) +
                   Convert.ToInt32(ViewSelected);
        }

        /// <summary>
        /// 	Compares two filter sets.
        /// </summary>
        /// <param name = "a">First filter set.</param>
        /// <param name = "b">Second filter set.</param>
        /// <returns>true if the sets are equal and false otherwise.</returns>
        public static bool operator ==(CompanyPerformanceFilterSettings a, CompanyPerformanceFilterSettings b)
        {
            return ReferenceEquals(a, b) || (!ReferenceEquals(a, null) && a.Equals(b));
        }

        /// <summary>
        /// 	Compares two filter sets.
        /// </summary>
        /// <param name = "a">First filter set.</param>
        /// <param name = "b">Second filter set.</param>
        /// <returns>true if the sets are not equal and false otherwise.</returns>
        public static bool operator !=(CompanyPerformanceFilterSettings a, CompanyPerformanceFilterSettings b)
        {
            return !ReferenceEquals(a, b) && (ReferenceEquals(a, null) || !a.Equals(b));
        }

        #endregion
    }
}
