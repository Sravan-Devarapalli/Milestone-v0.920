
namespace PraticeManagement
{
    /// <summary>
    /// Provides a generally used constants
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Contains the names of the HTML attributes.
        /// </summary>
        public static class HtmlAttributes
        {
            public const string OnDblClick = "ondblclick";
        }

        public static class ResourceKeys
        {
            # region Report

            public const string StartDateKey = "StartDateKey";
            public const string GranularityKey = "GranularityKey";
            public const string PeriodKey = "PeriodKey";
            public const string ProjectedPersonsKey = "ProjectedPersonsKey";
            public const string ProjectedProjectsKey = "ProjectedProjectsKey";
            public const string ActivePersonsKey = "ActivePersonsKey";
            public const string ActiveProjectsKey = "ActiveProjectsKey";
            public const string ExperimentalProjectsKey = "ExperimentalProjectsKey";
            public const string TimescaleIdListKey = "TimescaleIdListKey";
            public const string PracticeIdListKey = "PracticeIdListKey";
            public const string ExcludeInternalPracticesKey = "ExcludeInternalPracticesKey";
            public const string InternalProjectsKey = "InternalProjectsKey";
            public const string SortIdKey = "SortIdKey";
            public const string SortDirectionKey = "SortDirectionKey";
            public const string AvgUtilKey = "AvgUtilKey";
            public const string EndDateKey = "EndDateKey";

            # endregion Report

            # region SMTP

            public const string MailServerKey = "MailServer";
            public const string PortNumberKey = "PortNumber";
            public const string SSLEnabledKey = "SSLEnabled";
            public const string SMTPAuthRequiredKey = "SMTPAuthRequired";
            public const string UserNameKey = "UserName";
            public const string PasswordKey = "Password";
            public const string PMSupportEmailAddressKey = "PMSupportEmailAddress";

            # endregion SMTP

            # region Application

            public const string TimeZoneKey = "TimeZone";
            public const string IsDayLightSavingsTimeEffectKey = "IsDayLightSavingsTimeEffect";

            #endregion

        }

        /// <summary>
        /// Contains the names of the application's pages
        /// </summary>
        public static class ApplicationPages
        {
            public const string AppRootUrl = "~/";
            public const string LoginPage = "~/Login.aspx";
            public const string PersonList = "~/PersonList.aspx";
            public const string PersonDetail = "~/PersonDetail.aspx";
            public const string PersonMargin = "~/PersonMargin.aspx";
            public const string ProjectDetail = "~/ProjectDetail.aspx";
            public const string ClientList = "~/Config/Clients.aspx";
            public const string ClientDetails = "~/ClientDetails.aspx";
            public const string Reports = "~/Reports.aspx";
            public const string ConsultantsUtilizationReport = "~/Reports/UtilizationTable.aspx";
            public const string ConsTimelineReport = "~/Reports/UtilizationTimeline.aspx";
            public const string ConsTimelineReportDetails = "~/Reports/UtilizationTimeline.aspx#details";
            public const string MilestoneDetail = "~/MilestoneDetail.aspx";
            public const string MilestonePersonDetail = "~/MilestonePersonDetail.aspx";
            public const string PersonOverheadCalculation = "~/PersonOverheadCalculation.aspx";
            public const string OverheadDetail = "~/OverheadDetail.aspx";
            public const string Calendar = "~/Calendar.aspx";
            public const string Projects = "~/Projects.aspx";
            public const string CompensationDetail = "~/CompensationDetail.aspx";
            public const string DefaultRecruitingCommissionDetail = "~/DefaultRecruitingCommissionDetail.aspx";
            public const string ExpenseDetail = "~/ExpenseDetail.aspx";
            public const string ExpenseCategoryList = "~/ExpenseCategoryList.aspx";
            public const string OpportunityDetail = "~/OpportunityDetail.aspx";
            public const string OpportunityList = "~/OpportunityList.aspx";
            public const string TimeEntryForAdmin = "~/TimeEntry.aspx?day={0}&SelectedPersonId={1}";
            public const string TimeEntry = "~/TimeEntry.aspx?day={0}";
            public const string DetailsWithPrevNext = "{0}?id={1}&index={2}";
            public const string DetailRedirectFormat = "{0}?id={1}";
            public const string DetailRedirectWithReturnFormat = "{0}?id={1}&returnTo={2}";
            public const string ProjectDetailRedirectWithReturnFormat = "{0}?clientid={1}";
            public const string RedirectPersonIdFormat = "{0}?id={1}&personId={2}";
            public const string RedirectOpportunityIdFormat = "{0}?id={1}&opportunityId={2}";
            public const string RedirectMilestonePersonIdFormat = "{0}?id={1}&milestonePersonId={2}";
            public const string RedirectMilestonePersonIdFormatWithReturn = "{0}?id={1}&milestonePersonId={2}&returnTo={3}";
            public const string RedirectStartDateFormat = "{0}?id={1}&startDate={2}";
            public const string MilestonePrevNextRedirectFormat = "{0}?id={1}&projectId={2}"; // &returnTo={3}";
            public const string MilestoneWithReturnFormat = "{0}?id={1}&projectId={2}&returnTo={3}";
            public const string TimeEntryReport = "~/Reports/TimeEntryReport.aspx";
            public const string PageHasBeenRemoved = "~/GuestPages/PageHasBeenRemoved.aspx";
            public const string PageNotFound = "~/GuestPages/PageNotFound.aspx";
            public const string UtilizationTimelineWithDetails = "~/Reports/UtilizationTimeline.aspx#details";
            public const string ClientDetailsWithReturnFormat = "{0}?{1}&Id={2}";
            public const string ClientDetailsWithoutClientIdFormat = "{0}?{1}";
            public const string ChangePasswordErrorpage = "~/GuestPages/ChangePasswordError.aspx";
        }

        /// <summary>
        /// Contains the names of the application controls pages
        /// </summary>
        public static class ApplicationControls
        {
            public const string ProjectNameCellControl = "~/Controls/ProjectNameCell.ascx";
            public const string ProjectNameCellRoundedControl = "~/Controls/ProjectNameCellRounded.ascx";
        }

        /// <summary>
        /// Contains the names of the application resource files
        /// </summary>
        public static class ApplicationResources
        {
            public const string AddCommentIcon = "~/Images/balloon-plus.png";
            public const string RecentCommentIcon = "~/Images/balloon-ellipsis.png";
        }

        /// <summary>
        /// Contains the names of the report templates.
        /// </summary>
        public static class ReportTemplates
        {
            public const string MonthMiniReport = "~/Reports/Xslt/MonthMiniReport.xslt";
        }

        public static class Dates
        {
            public const int FirstMonth = 1;
            public const int LastMonth = 12;
            public const int FirstDay = 1;
            public const int WorkDaysInMonth = 20;
            public const int DefaultViewableMonths = 2;
            public const string ValidMinDate = "1/1/1975";
            public const string ValidMaxDate = "12/31/2100";
            public const int FYFirstMonth = 1;
            public const int FYLastMonth = 12;
        }

        /// <summary>
        /// Contains the formatting strings
        /// </summary>
        public static class Formatting
        {
            public const string IntegerNumberFormat = "{0:##############0}";
            public const string CurrencyZero = "$0.00";
            public const string MonthYearFormat = "MMM yyyy";
            public const string CompPerfMonthYearFormat = "MMM-yy";
            public const string SystemCurrencyFormat = "c";
            public const string TwoDecimalsFormat = "#######0.##";
            public const string PercentageFormat = "{0:##0.0#}%";
            public const string SearchResultFormat = "<span class=\"found\">$0</span>";
            public const string EntryDateFormat = "MM/dd/yyyy";
            public const string DoubleFormat = "F2";
            public const string CurrentVersionFormat = "Binaries: v{0}.{1}.{2}.{3} [{4:%y-MM-dd}] | Database: {5}";
            public const string Ellipsis = "...";
            public const string UnknownValue = "?";
            public const string GreetingUserName = "{0} {1}";
        }

        public static class HttpHeaders
        {
            public const string CacheControlNoCache = "no-cache";
        }

        /// <summary>
        /// Activity log XML strings
        /// </summary>
        public static class ActityLog
        {
            public const string ErrorLogMessage = @"<Error><NEW_VALUES	Login = ""{0}"" SourcePage = ""{1}"" SourceQuery = ""{2}""  ExcMsg=""{3}"" ExcSrc=""{4}"" InnerExcMsg=""{5}"" InnerExcSrc=""{6}""><OLD_VALUES /></NEW_VALUES></Error>";
            public const string ExportLogMessage = @"<Export><NEW_VALUES User = ""{0}"" From=""{1}""></NEW_VALUES><OLD_VALUES /></Export>";

            public const int ErrorMessageId = 20;
            public const int ExportMessageId = 6;
        }

        public static class QueryStringParameterNames
        {
            public const string Id = "id";
            public const string ReturnUrl = "returnTo";
            public const string RedirectUrlArgument = "redirectUrl";
            public const string QueryStringSeparator = "?";
            public const string RedirectFormat = "{0}?returnTo={1}";
            public const string RedirectWithQueryStringFormat = "{0}&returnTo={1}";
            public const string ActiveOnly = "activeOnly";
            public const string ClientId = "clientId";
            public const string SalesId = "salesId";
            public const string Index = "index";
            public const string SortOrder = "sortOrder";
            public const string SortDirection = "sortDirection";
        }

        public static class MethodParameterNames
        {
            public const string ID = "id";
            public const string MILESTONE_PERSON_ID = "milestonePersonId";
            public const string MODIFIED_BY_ID = "modifiedById";
            public const string PERSON_ID = "personId";
            public const string REQUESTER_ID = "requesterId";
            public const string SORT_EXPRESSION = "sortExpression";
            public const string TIME_TYPE_ID = "timeTypeId";
            public const string USER_NAME = "userName";
        }

        public static class ControlNames
        {
            public const string DDL_PROJECT_MILESTONES_EDIT = "ddlProjectMilestonesEdit";
            public const string DDL_TIMETYPE_EDIT = "ddlTimeTypeEdit";
            public const string HF_PERSON = "hfPerson";
        }

        public static class CssClassNames
        {
            public static string DIMMED_ROW = "declined-row";
        }

        public static class EntityNames
        {
            public const string IsChargeableEntity = "IsChargeable";
            public const string IsCorrectEntity = "IsCorrect";
            public const string ReviewStatusEntity = "ReviewStatus";
        }

        public static class Scripts
        {
            public const string CheckDirtyWithPostback = "if (showDialod()) {{{0}; return false; }} if(checkhdnchbActive())return true; return false;";
            public const string GoBack = "history.back();return false;";
        }
    }
}

