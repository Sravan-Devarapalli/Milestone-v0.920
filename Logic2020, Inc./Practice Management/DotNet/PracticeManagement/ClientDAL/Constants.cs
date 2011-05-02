namespace DataAccess
{
    internal class Constants
    {
        #region Nested type: ColumnNames

        /// <summary>
        /// Result set column names
        /// </summary>
        public class ColumnNames
        {
            public const string HoursInPeriod = "HoursInPeriod";
            public const string ProjectDiscount = "ProjectDiscount";
            public const string GrossHourlyBillRate = "GrossHourlyBillRate";
            public const string LoadedHourlyPayRate = "LoadedHourlyPayRate";
            public const string Id = "Id";
            public const string Date = "Date";
            public const string DayOff = "DayOff";
            public const string CompanyDayOff = "CompanyDayOff";
            public const string ReadOnly = "ReadOnly";
            public const string TargetId = "TargetId";
            public const string NoteId = "NoteId";
            public const string CreateDate = "CreateDate";
            public const string OwnerIdColumn = "OwnerId";
            public const string OwnerLastNameColumn = "OwnerLastName";
            public const string OwnerFirstNameColumn = "OwnerFirstName";
            public const string ActualHours = "ActualHours";
            public const string ActualHoursFrom = "ActualHoursFrom";
            public const string ActualHoursTo = "ActualHoursTo";
            public const string Alias = "Alias";
            public const string Amount = "Amount";
            public const string ClientId = "ClientId";
            public const string ClientIsChargeable = "ClientIsChargeable";
            public const string ClientName = "ClientName";
            public const string ConsultantsCanAdjust = "ConsultantsCanAdjust";
            public const string DefaultMpId = "DefaultMpId";
            public const string Discount = "Discount";
            public const string EndDate = "EndDate";
            public const string EntryDate = "EntryDate";
            public const string EntryDateFrom = "EntryDateFrom";
            public const string EntryDateTo = "EntryDateTo";
            public const string ExpectedHours = "ExpectedHours";
            public const string Expense = "Expense";
            public const string ReimbursedExpense = "ReimbursedExpense";
            public const string ExpenseId = "ExpenseId";
            public const string ExpenseName = "ExpenseName";
            public const string ExpenseAmount = "ExpenseAmount";
            public const string ExpenseReimbursement = "ExpenseReimbursement";
            public const string FirstName = "FirstName";
            public const string ForecastedHours = "ForecastedHours";
            public const string ForecastedHoursFrom = "ForecastedHoursFrom";
            public const string ForecastedHoursTo = "ForecastedHoursTo";
            public const string HoursPerDay = "HoursPerDay";
            public const string InUse = "InUse";
            public const string IsActive = "IsActive";
            public const string IsChargeable = "IsChargeable";
            public const string IsCompanyInternal = "IsCompanyInternal";
            public const string IsCorrect = "IsCorrect";
            public const string IsDefault = "IsDefault";
            public const string IsDefaultManager = "IsDefaultManager";
            public const string IsHourlyAmount = "IsHourlyAmount";
            public const string IsReviewed = "IsReviewed";
            public const string LastName = "LastName";
            public const string LastUpdateColumn = "LastUpdate";
            public const string ManagerAlias = "ManagerAlias";
            public const string ManagerFirstName = "ManagerFirstName";
            public const string ManagerId = "ManagerId";
            public const string ManagerLastName = "ManagerLastName";
            public const string MilestoneActualDeliveryDate = "MilestoneActualDeliveryDate";
            public const string MilestoneDate = "MilestoneDate";
            public const string MilestoneDateFrom = "MilestoneDateFrom";
            public const string MilestoneDateTo = "MilestoneDateTo";
            public const string MilestoneExpectedHours = "MilestoneExpectedHours";
            public const string MilestoneHourlyRevenue = "MilestoneHourlyRevenue";
            public const string MilestoneId = "MilestoneId";
            public const string MilestoneIsChargeable = "MilestoneIsChargeable";
            public const string MilestoneName = "MilestoneName";
            public const string MilestonePersonId = "MilestonePersonId";
            public const string MilestonePersonFirstName = "MilestonePersonFirstName";
            public const string MilestonePersonLastName = "MilestonePersonLastName";
            public const string MonthStartDate = "MonthStartDate";
            public const string MonthEndDate = "MonthEndDate";
            public const string MilestoneProjectedDeliveryDate = "MilestoneProjectedDeliveryDate";
            public const string MilestoneStartDate = "MilestoneStartDate";
            public const string ModifiedBy = "ModifiedBy";
            public const string ModifiedByFirstName = "ModifiedByFirstName";
            public const string ModifiedByLastName = "ModifiedByLastName";
            public const string ModifiedDate = "ModifiedDate";
            public const string ModifiedDateFrom = "ModifiedDateFrom";
            public const string ModifiedDateTo = "ModifiedDateTo";
            public const string Name = "Name";
            public const string Note = "Note";
            public const string Notes = "Notes";
            public const string ObjectFirstName = "ObjectFirstName";
            public const string ObjectLastName = "ObjectLastName";
            public const string PersonId = "PersonId";
            public const string PersonName = "PersonName";
            public const string PersonRoleId = "PersonRoleId";
            public const string PersonRoleName = "RoleName";
            public const string PersonSeniorityId = "SeniorityId";
            public const string PersonVacationsOnMilestone = "VacationDays";
            public const string ProjectEndDate = "ProjectEndDate";
            public const string PracticesOwned = "PracticesOwned";
            public const string PracticeOwnerName = "PracticeOwnerName";
            public const string ProjectId = "ProjectId";
            public const string ProjectIdList = "ProjectIds";
            public const string ProjectIsChargeable = "ProjectIsChargeable";
            public const string ProjectManagerId = "ProjectManagerId";
            public const string ProjectManagerFirstName = "ProjectManagerFirstName";
            public const string ProjectManagerLastName = "ProjectManagerLastName";
            public const string ProjectName = "ProjectName";
            public const string ProjectStartDate = "ProjectStartDate";
            public const string ProjectStatusId = "ProjectStatusId";
            public const string SalesCommission = "SalesCommission";
            public const string PersonStatusName = "PersonStatusName";
            public const string PersonStatusId = "PersonStatusId";
            public const string SortExpression = "SortExpression";
            public const string StartDate = "StartDate";
            public const string TargetPersonId = "TargetPersonId";
            public const string TargetFirstName = "TargetFirstName";
            public const string TargetLastName = "TargetLastName";
            public const string TelephoneNumber = "TelephoneNumber";
            public const string TimeEntryId = "TimeEntryId";
            public const string TimeTypeId = "TimeTypeId";
            public const string TimeTypeName = "TimeTypeName";
            public const string TotalActualHours = "TotalActualHours";
            public const string TotalForecastedHours = "TotalForecastedHours";
            public const string GrossMarginColumn = "GrossMargin";
            public const string HoursColumn = "Hours";
            public const string SalesCommissionColumn = "SalesCommission";
            public const string PracticeManagementCommissionColumn = "PracticeManagementCommission";
            public const string DateColumn = "Date";
            public const string VirtualConsultantsColumn = "VirtualConsultants";
            public const string EmployeesNumberColumn = "EmployeesNumber";
            public const string ConsultantsNumberColumn = "ConsultantsNumber";
            public const string FinancialDateColumn = "FinancialDate";
            public const string RevenueColumn = "Revenue";
            public const string RevenueNetColumn = "RevenueNet";
            public const string CogsColumn = "Cogs";
            public const string VacationHours = "VacationHours";
            public const string ProjectIdColumn = "ProjectId";
            public const string ClientIdColumn = "ClientId";
            public const string DiscountColumn = "Discount";
            public const string TermsColumn = "Terms";
            public const string NameColumn = "Name";
            public const string PracticeManagerIdColumn = "PracticeManagerId";
            public const string EstimatedRevenueColumn = "EstimatedRevenue";
            public const string PracticeManagerFirstNameColumn = "PracticeManagerFirstName";
            public const string PracticeManagerLastNameColumn = "PracticeManagerLastName";
            public const string PracticeIdColumn = "PracticeId";
            public const string PracticeNameColumn = "PracticeName";
            public const string DirectorFirstNameColumn = "DirectorFirstName";
            public const string DirectorLastNameColumn = "DirectorLastName";
            public const string DirectorIdColumn = "DirectorId";
            public const string DirectorStatusIdColumn = "DirectorStatusId";
            public const string DirectorStatusNameColumn = "DirectorStatusName";
            public const string ClientNameColumn = "ClientName";
            public const string StartDateColumn = "StartDate";
            public const string EndDateColumn = "EndDate";
            public const string ProjectStatusIdColumn = "ProjectStatusId";
            public const string ProjectStatusNameColumn = "ProjectStatusName";
            public const string ProjectNumberColumn = "ProjectNumber";
            public const string MilestoneIdColumn = "MilestoneId";
            public const string ProjectNameColumn = "ProjectName";
            public const string DescriptionColumn = "Description";
            public const string ProjectStartDateColumn = "ProjectStartDate";
            public const string ProjectEndDateColumn = "ProjectEndDate";
            public const string BuyerNameColumn = "BuyerName";
            public const string OpportunityIdColumn = "OpportunityId";
            public const string ProjectGroupIdColumn = "GroupId";
            public const string ProjectGroupNameColumn = "GroupName";
            public const string ProjectGroupInUseColumn = "InUse";
            public const string SystemUser = "SystemUser";
            public const string Workstation = "Workstation";
            public const string ApplicationName = "ApplicationName";
            public const string UserLogin = "UserLogin";
            public const string LogData = "LogData";
            public const string ActivityName = "ActivityName";
            public const string ActivityId = "ActivityID";
            public const string ActivityTypeId = "ActivityTypeID";
            public const string SessionId = "SessionID";
            public const string LogDate = "LogDate";
            public const string OpportunityTransitionId = "OpportunityTransitionId";
            public const string OpportunityTransitionStatusId = "OpportunityTransitionStatusId";
            public const string TransitionDate = "TransitionDate";
            public const string NoteText = "NoteText";
            public const string OpportunityTransitionStatusName = "OpportunityTransitionStatusName";
            public const string SalespersonIdColumn = "SalespersonId";
            public const string OpportunityStatusIdColumn = "OpportunityStatusId";
            public const string PriorityColumn = "Priority";
            public const string ProjectedStartDateColumn = "ProjectedStartDate";
            public const string ProjectedEndDateColumn = "ProjectedEndDate";
            public const string OpportunityNumberColumn = "OpportunityNumber";
            public const string OpportunityName = "OpportunityName";
            public const string SalespersonFirstNameColumn = "SalespersonFirstName";
            public const string SalespersonLastNameColumn = "SalespersonLastName";
            public const string SalespersonFullNameColumn = "SalespersonName";
            public const string SalespersonStatusColumn = "SalespersonStatus";
            public const string CommissionTypeColumn = "CommissionType";
            public const string OpportunityStatusNameColumn = "OpportunityStatusName";
            public const string CreateDateColumn = "CreateDate";
            public const string PipelineColumn = "Pipeline";
            public const string ProposedColumn = "Proposed";
            public const string SendOutColumn = "SendOut";
            public const string RevenueTypeColumn = "RevenueType";
            public const string OpportunityIndexColumn = "OpportunityIndex";
            public const string IsProjectChargeable = "IsProjectChargeable";
            public const string OwnerStatusColumn = "OwnerStatus";
            public const string HireDateColumn = "HireDate";
            public const string TerminationDateColumn = "TerminationDate";
            public const string SettingsKeyColumn = "SettingsKey";
            public const string ValueColumn = "Value";
        }

        #endregion

        #region Nested type: FunctionNames

        public class FunctionNames
        {
            public const string IsSomeonesManager = "dbo.IsSomeonesManager";
        }

        #endregion

        #region Nested type: ParameterNames

        /// <summary>
        /// Stored procedures parameter names
        /// </summary>
        public class ParameterNames
        {
            public const string OpportunityTransitionId = ColumnNames.OpportunityTransitionId;
            public const string TargetPerson = "@TargetPersonId";
            public const string NoteTargetId = "@NoteTargetId";
            public const string NoteId = "@NoteId";
            public const string TargetId = "@TargetId";
            public const string SortId = "@SortId";
            public const string SortDirection = "@SortDirection";
            public const string ExcludeInternalPractices = "@ExcludeInternalPractices";
            public const string IsSampleReport = "@IsSampleReport";
            public const string TimescaleIds = "TimescaleIds";
            public const string ActivePersons = "ActivePersons";
            public const string ProjectedPersons = "ProjectedPersons";
            public const string ActiveProjects = "ActiveProjects";
            public const string ProjectedProjects = "ProjectedProjects";
            public const string ExperimentalProjects = "ExperimentalProjects";
            public const string CompletedProjects = "CompletedProjects";
            public const string InternalProjects = "InternalProjects";
            public const string IncludeOverheads = "@IncludeOverheads";
            public const string IncludeZeroCostEmployees = "@IncludeZeroCostEmployees";
            public const string Granularity = DaysForward;
            public const string Period = Step;
            public const string Start = StartDate;
            public const string End = EndDate;
            public const string ActualHours = ColumnNames.ActualHours;
            public const string ActualHoursFrom = ColumnNames.ActualHoursFrom;
            public const string ActualHoursTo = ColumnNames.ActualHoursTo;
            public const string Alias = ColumnNames.Alias;
            public const string ClientId = ColumnNames.ClientId;
            public const string ClientIsChargeable = ColumnNames.ClientIsChargeable;
            public const string ClientName = ColumnNames.ClientName;
            public const string ClonedProjectId = "ClonedProjectId";
            public const string CloneMilestones = "CloneMilestones";
            public const string CloneBillingInfo = "CloneBillingInfo";
            public const string CloneBillingNotes = "CloneBillingNotes";
            public const string CloneNotes = "CloneNotes";
            public const string CloneCommissions = "CloneCommissions";
            public const string ConsultantsCanAdjust = ColumnNames.ConsultantsCanAdjust;
            public const string CurrentId = "CurrentId";
            public const string DefaultManagerId = "DefaultManagerId";
            public const string DefaultMpId = ColumnNames.DefaultMpId;
            public const string EndDate = ColumnNames.EndDate;
            public const string EntryDate = ColumnNames.EntryDate;
            public const string EntryDateFrom = ColumnNames.EntryDateFrom;
            public const string EntryDateTo = ColumnNames.EntryDateTo;
            public const string ExpenseId = ColumnNames.ExpenseId;
            public const string ExpenseName = ColumnNames.ExpenseName;
            public const string ExpenseAmount = ColumnNames.ExpenseAmount;
            public const string ExpenseReimbursement = ColumnNames.ExpenseReimbursement;
            public const string ForecastedHours = ColumnNames.ForecastedHours;
            public const string ForecastedHoursFrom = ColumnNames.ForecastedHoursFrom;
            public const string ForecastedHoursTo = ColumnNames.ForecastedHoursTo;
            public const string HoursPerDay = ColumnNames.HoursPerDay;
            public const string IncludeActive = "IncludeActive";
            public const string IncludeProjected = "IncludeProjected";
            public const string IncludeInactive = "IncludeInactive";
            public const string IncludeInternal = "IncludeInternal";
            public const string IncludeExperimental = "includeExperimental";
            public const string IncludeCompleted = "includeCompleted";
            public const string IncludeDefaultMileStone = "@IncludeDefaultMileStone";
            public const string IsCompanyInternal = ColumnNames.IsCompanyInternal;
            public const string IncludeTotals = "IncludeTotals";
            public const string InUse = ColumnNames.InUse;
            public const string IsActive = ColumnNames.IsActive;
            public const string IsChargeable = ColumnNames.IsChargeable;
            public const string IsCorrect = ColumnNames.IsCorrect;
            public const string IsProjectChargeable = ColumnNames.IsProjectChargeable;
            public const string IsDefault = ColumnNames.IsDefault;
            public const string IsReviewed = ColumnNames.IsReviewed;
            public const string ManagerAlias = ColumnNames.ManagerAlias;
            public const string ManagerFirstName = ColumnNames.ManagerFirstName;
            public const string ManagerId = ColumnNames.ManagerId;
            public const string ManagerLastName = ColumnNames.ManagerLastName;
            public const string MilestoneDate = ColumnNames.MilestoneDate;
            public const string MilestoneDateFrom = ColumnNames.MilestoneDateFrom;
            public const string MilestoneDateTo = ColumnNames.MilestoneDateTo;
            public const string MilestoneFrom = ColumnNames.MilestoneDateFrom;
            public const string MilestoneId = ColumnNames.MilestoneId;
            public const string MilestoneIsChargeable = ColumnNames.MilestoneIsChargeable;
            public const string MilestoneName = ColumnNames.MilestoneName;
            public const string MilestonePersonId = ColumnNames.MilestonePersonId;
            public const string MilestoneTo = ColumnNames.MilestoneDateTo;
            public const string ModifiedBy = ColumnNames.ModifiedBy;
            public const string ModifiedDate = ColumnNames.ModifiedDate;
            public const string ModifiedDateFrom = ColumnNames.ModifiedDateFrom;
            public const string ModifiedDateTo = ColumnNames.ModifiedDateTo;
            public const string ModifiedFirstName = ColumnNames.ModifiedByFirstName;
            public const string ModifiedLastName = ColumnNames.ModifiedByLastName;
            public const string Name = ColumnNames.Name;
            public const string NewManagerId = "NewManagerId";
            public const string Note = ColumnNames.Note;
            public const string Notes = ColumnNames.Notes;
            public const string ObjectFirstName = ColumnNames.ObjectFirstName;
            public const string ObjectLastName = ColumnNames.ObjectLastName;
            public const string OldManagerId = "OldManagerId";
            public const string PersonId = ColumnNames.PersonId;
            public const string PersonIds = "PersonIds";
            public const string PersonName = ColumnNames.PersonName;
            public const string ProjectId = ColumnNames.ProjectId;
            public const string ProjectIdList = ColumnNames.ProjectIdList;
            public const string ProjectIsChargeable = ColumnNames.ProjectIsChargeable;
            public const string ProjectName = ColumnNames.ProjectName;
            public const string ProjectNumber = ColumnNames.ProjectNumberColumn;
            public const string ProjectManagerId = ColumnNames.ProjectManagerId;
            public const string ProjectStatusId = ColumnNames.ProjectStatusId;
            public const string RequesterId = "RequesterId";
            public const string SortExpression = ColumnNames.SortExpression;
            public const string StartDate = ColumnNames.StartDate;
            public const string TimeEntryId = ColumnNames.TimeEntryId;
            public const string TimeTypeId = ColumnNames.TimeTypeId;
            public const string TimeTypeName = ColumnNames.TimeTypeName;
            public const string ProjectIdParam = "@ProjectId";
            public const string MilestoneIdParam = "@MilestoneId";
            public const string PersonIdParam = "@PersonId";
            public const string PersonIdListParam = "@PersonIdList";
            public const string StartDateParam = "@StartDate";
            public const string EndDateParam = "@EndDate";
            public const string YearParam = "@Year";
            public const string EntryStartDateParam = "@EntryStartDate";
            public const string EntryEndDateParam = "@EntryEndDate";
            public const string ClientIdsParam = "@ClientIds";
            public const string ClientIdParam = "@ClientId";
            public const string DiscountParam = "@Discount";
            public const string TermsParam = "@Terms";
            public const string NameParam = "@Name";
            public const string EstimatedRevenueParam = "@EstimatedRevenue";
            public const string OwnerId = ColumnNames.OwnerIdColumn;
            public const string ProjectOwnerIdsParam = "@ProjectOwnerIds";
            public const string Date = "Date";
            public const string DayOff = "DayOff";
            public const string PracticeManagerIdParam = "@PracticeManagerId";
            public const string DirecterIdParam = "@DirectorId";
            public const string SalespersonIdsParam = "@SalespersonIds";
            public const string SalespersonIdParam = "@SalespersonId";
            public const string PracticeIdsParam = "@PracticeIds";
            public const string PracticeIdParam = "@PracticeId";
            public const string ProjectGroupIdsParam = "@ProjectGroupIds";
            public const string ProjectGroupIdParam = "@ProjectGroupId";
            public const string ProjectStatusIdParam = "@ProjectStatusId";
            public const string ShowProjectedParam = "@ShowProjected";
            public const string ShowCompletedParam = "@ShowCompleted";
            public const string ShowActiveParam = "@ShowActive";
            public const string ShowInternalParam = "@showInternal";
            public const string ShowExperimentalParam = "@ShowExperimental";
            public const string ShowInactiveParam = "@showInactive";
            public const string UserParam = "@User";
            public const string LookedParam = "@Looked";
            public const string BuyerNameParam = "@BuyerName";
            public const string UserLoginParam = "@UserLogin";
            public const string OpportunityIdParam = "@OpportunityId";
            public const string GroupIdParam = "@GroupId";
            public const string Step = "@Step";
            public const string DaysForward = "@DaysForward";
            public const string PageSize = "@PageSize";
            public const string PageNo = "@PageNo";
            public const string EventSource = "@EventSource";
            public const string ActivityTypeId = "@ActivityTypeID";
            public const string LogData = "@LogData";
            public const string OpportunityTransitionStatusId = "@OpportunityTransitionStatusId";
            public const string NoteText = "@NoteText";
            public const string ActiveOnlyParam = "@ActiveOnly";
            public const string OpportunityStatusIdParam = "@OpportunityStatusId";
            public const string PriorityParam = "@Priority";
            public const string ProjectedStartDateParam = "@ProjectedStartDate";
            public const string ProjectedEndDateParam = "@ProjectedEndDate";
            public const string DescriptionParam = "@Description";
            public const string PipelineParam = "@Pipeline";
            public const string ProposedParam = "@Proposed";
            public const string SendOutParam = "@SendOut";
            public const string RevenueTypeParam = "@RevenueType";
            public const string OpportunityIndexParam = "@OpportunityIndex";
            public const string OpportunitySortOrderParam = "@SortOrder";
            public const string OpportunitySortDirectionParam = "@SortDirection";
            public const string DefaultMilestoneId = "@DefaultMilestoneId";
            public const string SortByParam = "@SortBy";
            public const string CategoryTypeIdParam = "@CategoryTypeId";
            public const string MonthStartDateParam = "@MonthStartDate";
            public const string AmountParam = "@Amount";
            public const string ItemIdParam = "@ItemId";
            public const string ItemIdsParam = "@ItemIds";
            public const string SettingsTypeParam = "@TypeId";
            public const string SettingsKeyParam = "@SettingsKey";
            public const string ValueParam = "@Value";
            public const string HasPersons = "@HasPersons";
            public const string CategoryItemsXMLParam = "@CategoryItemsXML";
            public const string IsOnlyActiveAndProjective = "@IsOnlyActiveAndProjective";

        }

        #endregion

        #region Nested type: ProcedureNames

        /// <summary>
        /// Stored procedure names
        /// </summary>
        public class ProcedureNames
        {
            public class ActivityLog
            {
                public const string ActivityLogListByPeriodProcedure = "dbo.ActivityLogListByPeriod";
                public const string ActivityLogGetCountProcedure = "dbo.ActivityLogGetCount";
                public const string UserActivityLogInsertProcedure = "dbo.UserActivityLogInsert";
                public const string GetDatabaseVersionFunction = "SELECT dbo.GetDatabaseVersion()";
            }

            #region Nested type: ComputedFinancials

            public class ComputedFinancials
            {
                public const string FinancialsListByProjectPeriod = "dbo.FinancialsListByProjectPeriod";
                public const string FinancialsListByProjectPeriodTotal = "dbo.FinancialsListByProjectPeriodTotal";
                public const string FinancialsGetByProject = "dbo.FinancialsGetByProject";
                public const string FinancialsGetByMilestonePersonPeriod = "dbo.FinancialsGetByMilestonePersonPeriod";
                public const string FinancialsGetByMilestonePersonEntry = "dbo.FinancialsGetByMilestonePersonEntry";
                public const string FinancialsGetByMilestone = "dbo.FinancialsGetByMilestone";
                public const string FinancialsGetByMilestonePerson = "dbo.FinancialsGetByMilestonePerson";
                public const string PersonStatsByDate = "dbo.PersonStatsByDateRange";
                public const string FinancialsGetByMilestonePersonsMonthly = "dbo.FinancialsGetByMilestonePersonsMonthly";
                public const string FinancialsGetByMilestonePersonsTotal = "dbo.FinancialsGetByMilestonePersonsTotal";
                public const string CalculateMilestonePersonFinancials = "dbo.CalculateMilestonePersonFinancials";

            }

            #endregion

            #region Nested type: Configuration

            public class Configuration
            {
                public const string GetCompanyLogoDataProcedure = "dbo.GetCompanyLogoData";
                public const string CompanyLogoDataSaveProcedure = "dbo.CompanyLogoDataSave";
                public const string SaveSettingsKeyValuePairsProcedure = "dbo.SaveSettingsKeyValuePairs";
                public const string GetSettingsKeyValuePairsProcedure = "dbo.GetSettingsKeyValuePairsBySettingsType";
            }

            #endregion

            #region Nested type: Person

            public class Person
            {
                public const string SetNewManager = "dbo.SetNewManager";
                public const string SetDefaultManager = "dbo.PersonSetDefaultManager";
                public const string ListManagersSubordinates = "dbo.ListManagersSubordinates";
                public const string PersonListByCategoryTypeAndPeriod = "[dbo].[PersonListByCategoryTypeAndPeriod]";
            }

            #endregion

            #region Nested type: TimeEntry

            public class TimeType
            {
                public const string GetAll = "dbo.TimeTypeGetAll";
                public const string Update = "dbo.TimeTypeUpdate";
                public const string Insert = "dbo.TimeTypeInsert";
                public const string Delete = "dbo.TimeTypeDelete";
            }

            public class TimeEntry
            {
                #region Time entry

                public const string Add = "dbo.TimeEntryInsert";
                public const string Remove = "dbo.TimeEntryRemove";
                public const string RemoveTimeEntries = "dbo.TimeEntriesRemove";
                public const string Get = "dbo.PersonTimeEntries";
                public const string Update = "dbo.TimeEntryUpdate";
                public const string ListAll = "dbo.TimeEntriesAll";
                public const string GetCount = "dbo.TimeEntriesGetCount";
                public const string GetTotals = "dbo.TimeEntriesGetTotals";

                public const string ToggleIsReviewed = "dbo.TimeEntryToggleIsReviewed";
                public const string ToggleIsCorrect = "dbo.TimeEntryToggleIsCorrect";
                public const string ToggleIsChargeable = "dbo.TimeEntryToggleIsChargeable";

                #endregion

                #region Filters

                public const string TimeEntryAllPersons = "dbo.TimeEntryAllPersons";
                public const string TimeEntryAllMilestones = "dbo.TimeEntryAllMilestones";

                #endregion

                #region Shared

                public const string ConsultantMilestones = "dbo.ConsultantMilestones";
                public const string GetTimeEntryMilestones = "dbo.GetTimeEntryMilestones";
                #endregion

                #region Reports

                public const string TimeEntriesGetByProject = "dbo.TimeEntriesGetByProject";
                public const string TimeEntriesGetByPerson = "dbo.TimeEntriesGetByPerson";
                public const string TimeEntriesGetByManyPersons = "dbo.TimeEntriesGetByManyPersons";
                public const string TimeEntriesGetByProjectCumulative = "dbo.TimeEntryHoursByPersonProject";
                public const string TimeEntryHoursByManyPersonsProject = "dbo.TimeEntryHoursByManyPersonsProject";

                #endregion
            }

            #endregion

            public class Practices
            {
                public const string GetAll = "dbo.PracticeListAll";
                public const string GetById = "dbo.PracticeGetById";
                public const string Update = "dbo.PracticeUpdate";
                public const string Insert = "dbo.PracticeInsert";
                public const string Delete = "dbo.PracticeDelete";
            }

            public class Calendar
            {
                public const string CalendarGetProcedure = "dbo.CalendarGet";
                public const string CalendarUpdateProcedure = "dbo.CalendarUpdate";
                public const string WorkDaysCompanyNumberProcedure = "dbo.WorkDaysCompanyNumber";
                public const string WorkDaysPersonNumberProcedure = "dbo.WorkDaysPersonNumber";
                public const string GetCompanyHolidaysProcedure = "dbo.GetCompanyHolidays";
            }

            public class ProjectGroup
            {
                public const string ProjectGroupListAll = "dbo.ProjectGroupListAll";
                public const string ProjectGroupRenameByClient = "dbo.ProjectGroupRenameByClient";
                public const string ProjectGroupInsert = "dbo.ProjectGroupInsert";
                public const string ProjectGroupDelete = "dbo.ProjectGroupDelete";
            }

            public class Note
            {
                public const string NoteGetByTargetId = "dbo.NoteGetByTargetId";
                public const string NoteInsert = "dbo.NoteInsert";
                public const string NoteUpdate = "dbo.NoteUpdate";
                public const string NoteDelete = "dbo.NoteDelete";
            }

            public class Project
            {
                public const string ProjectListAll = "dbo.ProjectListAll";
                public const string ProjectListAllMultiParameters = "dbo.ProjectListAllMultiParameters";
                public const string ProjectsListByClient = "dbo.ProjectsListByClient";
                public const string ListProjectsByClientShort = "dbo.ListProjectsByClientShort";
                public const string ProjectsListByClientWithSort = "dbo.ProjectsListByClientWithSort";
                public const string ProjectsCountByClient = "dbo.ProjectsCountByClient";
                public const string ProjectGetById = "dbo.ProjectGetById";
                public const string ProjectInsert = "dbo.ProjectInsert";
                public const string ProjectUpdate = "dbo.ProjectUpdate";
                public const string ProjectSetStatus = "dbo.ProjectSetStatus";
                public const string ProjectSearchText = "dbo.ProjectSearchText";
                public const string CloneProject = "dbo.CloneProject";
                public const string ProjectGetByNumber = "dbo.ProjectGetByNumber";
                public const string ProjectMilestonesFinancials = "dbo.ProjectMilestonesFinancials";
                public static string GetProjectListWithFinancials = "dbo.GetProjectListWithFinancials";
                public static string GetProjectListForGroupingPracticeManagers = "dbo.GetProjectListForGroupingPracticeManagers";
                public const string CategoryItemBudgetSave = "dbo.CategoryItemBudgetSave";
                public const string CategoryItemListByCategoryType = "dbo.CategoryItemListByCategoryType";
                public const string CalculateBudgetForCategoryItems = "dbo.CalculateBudgetForCategoryItems";
                public const string CategoryItemsSaveFromXML = "dbo.CategoryItemsSaveFromXML";
            }

            public class MilestonePerson
            {
                public const string ConsultantMilestones = "dbo.ConsultantMilestones";
                public const string MilestonePersonListByProject = "dbo.MilestonePersonListByProject";
                public const string MilestonePersonListByProjectShort = "dbo.MilestonePersonListByProjectShort";
                public const string MilestonePersonListByMilestone = "dbo.MilestonePersonListByMilestone";
                public const string MilestonePersonGetByMilestonePerson = "dbo.MilestonePersonGetByMilestonePerson";
                public const string MilestonePersonInsert = "dbo.MilestonePersonInsert";
                public const string MilestonePersonDelete = "dbo.MilestonePersonDelete";
                public const string MilestonePersonEntryInsert = "dbo.MilestonePersonEntryInsert";
                public const string MilestonePersonDeleteEntries = "dbo.MilestonePersonDeleteEntries";
                public const string MilestonePersonListByProjectMonth = "dbo.MilestonePersonListByProjectMonth";
                public const string MilestonePersonListByProjectPerson = "dbo.MilestonePersonListByProjectPerson";
                public const string MilestonePersonListByPerson = "dbo.MilestonePersonListByPerson";
                public const string MilestonePersonGetByMilestonePersonId = "dbo.MilestonePersonGetByMilestonePersonId";
                public const string MilestonePersonEntryListByMilestonePersonId = "dbo.MilestonePersonEntryListByMilestonePersonId";
                public const string CheckTimeEntriesForMilestonePerson = "dbo.CheckTimeEntriesForMilestonePerson";
            }

            public class ProjectExpenses
            {
                public const string GetById = "dbo.ProjectExpenseGetById";
                public const string GetAllForMilestone = "dbo.ProjectExpenseGetAllForMilestone";
                public const string Update = "dbo.ProjectExpenseUpdate";
                public const string Insert = "dbo.ProjectExpenseInsert";
                public const string Delete = "dbo.ProjectExpenseDelete";
            }

            public class Opportunitites
            {
                public const string OpportunityTransitionGetByOpportunity = "dbo.OpportunityTransitionGetByOpportunity";
                public const string OpportunityTransitionGetByPerson = "dbo.OpportunityTransitionGetByPerson";
                public const string OpportunityTransitionInsert = "dbo.OpportunityTransitionInsert";
                public const string OpportunityTransitionDelete = "dbo.OpportunityTransitionDelete";
                public const string OpportunityListAll = "dbo.OpportunityListAll";
                public const string OpportunityListAllShort = "dbo.OpportunityListAllShort";
                public const string OpportunityGetById = "dbo.OpportunityGetById";
                public const string OpportunityInsert = "dbo.OpportunityInsert";
                public const string OpportunityUpdate = "dbo.OpportunityUpdate";
                public const string OpportunityGetExcelSet = "dbo.OpportunityExcelSet";
                public const string OpportunityConvertToProject = "dbo.OpportunityConvertToProject";
                public const string OpportunityGetByNumber = "dbo.OpportunityGetByNumber";
                public const string GetOpportunityPersons = "dbo.GetOpportunityPersons";
                public const string ConvertOpportunityToProject = "dbo.ConvertOpportunityToProject";
                public const string OpportunityPersonInsert = "dbo.OpportunityPersonInsert";
                public const string OpportunityPersonDelete = "dbo.OpportunityPersonDelete";
                public const string OpportunityPrioritiesListAll = "dbo.OpportunityPrioritiesListAll";
            }
        }

        #endregion

        #region Nested type: Queries

        public class Queries
        {
            public const string SingleParameter = "SELECT {0}(@{1})";
        }

        #endregion
    }
}

