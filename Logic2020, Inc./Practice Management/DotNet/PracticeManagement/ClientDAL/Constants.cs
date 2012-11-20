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

            public const string IsNotesRequired = "IsNotesRequired";
            public const string UploadedDate = "UploadedDate";
            public const string AttachmentSize = "AttachmentSize";
            public const string IsMarginColorInfoEnabledColumn = "IsMarginColorInfoEnabled";
            public const string ColorIdColumn = "ColorId";
            public const string StartRangeColumn = "StartRange";
            public const string EndRangeColumn = "EndRange";
            public const string IsWelcomeEmailSent = "IsWelcomeEmailSent";
            public const string W2Salary_RateColumn = "W2Salary_Rate";
            public const string W2Hourly_RateColumn = "W2Hourly_Rate";
            public const string _1099_Hourly_RateColumn = "_1099_Hourly_Rate";
            public const string FileName = "FileName";
            public const string HoursInPeriod = "HoursInPeriod";
            public const string ProjectDiscount = "ProjectDiscount";
            public const string GrossHourlyBillRate = "GrossHourlyBillRate";
            public const string LoadedHourlyPayRate = "LoadedHourlyPayRate";
            public const string Id = "Id";
            public const string EntryId = "EntryId";
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
            public const string ReviewStatusId = "ReviewStatusId";
            public const string LastName = "LastName";
            public const string ObjectId = "ObjectId";
            public const string ObjectName = "ObjectName";
            public const string ObjectNumber = "ObjectNumber";
            public const string ObjectType = "ObjectType";
            public const string ObjectStatusId = "ObjectStatusId";
            public const string LinkedObjectId = "LinkedObjectId";
            public const string LinkedObjectNumber = "LinkedObjectNumber";
            public const string QuantityString = "QuantityString";
            public const string LastUpdateColumn = "LastUpdate";
            public const string ManagerAlias = "ManagerAlias";
            public const string ManagerFirstName = "ManagerFirstName";
            public const string ManagerId = "ManagerId";
            public const string ManagerLastName = "ManagerLastName";
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
            public const string OpportunityPersonTypeId = "OpportunityPersonTypeId";
            public const string OpportunityPersonRelationTypeId = "RelationTypeId";
            public const string OpportunityPersonQuantity = "Quantity";
            public const string NeedBy = "NeedBy";
            public const string PersonName = "PersonName";
            public const string PersonRoleId = "PersonRoleId";
            public const string PersonRoleName = "RoleName";
            public const string PersonSeniorityId = "SeniorityId";
            public const string PersonSeniorityName = "SeniorityName";
            public const string PersonVacationsOnMilestone = "VacationDays";
            public const string ProjectEndDate = "ProjectEndDate";
            public const string PracticesOwned = "PracticesOwned";
            public const string PracticeOwnerName = "PracticeOwnerName";
            public const string ProjectId = "ProjectId";
            public const string ProjectIdList = "ProjectIds";
            public const string ProjectIsChargeable = "ProjectIsChargeable";
            public const string ProjectManagerId = "ProjectManagerId";
            public const string ProjectManagersIdFirstNameLastName = "ProjectManagersIdFirstNameLastName";
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
            public const string AttachmentDataColumn = "AttachmentData";
            public const string ClientIdColumn = "ClientId";
            public const string DiscountColumn = "Discount";
            public const string TermsColumn = "Terms";
            public const string NameColumn = "Name";
            public const string PracticeManagerIdColumn = "PracticeManagerId";
            public const string EstimatedRevenueColumn = "EstimatedRevenue";
            public const string OutSideResourcesColumn = "OutSideResources";
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
            public const string DisplayNameColumn = "DisplayName";
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
            public const string PrioritySortOrderColumn = "PrioritySortOrder";
            public const string PriorityIdColumn = "PriorityId";
            public const string ProjectedStartDateColumn = "ProjectedStartDate";
            public const string ProjectedEndDateColumn = "ProjectedEndDate";
            public const string CloseDateColumn = "CloseDate";
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
            public const string PriorityTrendTypeColumn = "PriorityTrendType";
            public const string PriorityTrendCountColumn = "PriorityTrendCount";
            public const string StatusColumn = "Status";
            public const string StatusCountColumn = "StatusCount";
            public const string IsSetColumn = "IsSet";
            public const string IsRecurringColumn = "IsRecurring";
            public const string RecurringHolidayIdColumn = "RecurringHolidayId";
            public const string HolidayDescriptionColumn = "HolidayDescription";
            public const string RecurringHolidayDateColumn = "RecurringHolidayDate";
            public const string IsFloatingHolidayColumn = "IsFloatingHoliday";
            public const string IsAllowedToEditColumn = "IsAllowedToEdit";
            public const string HasAttachmentsColumn = "HasAttachments";
            public const string DashBoardTypeIdColumn = "DashBoardTypeId";
            public const string LinkNameColumn = "LinkName";
            public const string VirtualPathColumn = "VirtualPath";
            public const string IsStrawmanColumn = "IsStrawman";
            public const string TimeEntrySectionId = "TimeEntrySectionId";
            public const string ChargeCodeId = "ChargeCodeId";
            public const string IsRecursive = "IsRecursive";
            public const string ChargeCodeDate = "ChargeCodeDate";
            public const string IsInternalColumn = "IsInternal";
            public const string CanCreateCustomWorkTypesColumn = "CanCreateCustomWorkTypes";
            public const string InFutureUse = "InFutureUse";
            public const string IsChargeCodeOffColumn = "IsChargeCodeOff";
            public const string IsHourlyRevenueColumn = "IsHourlyRevenue";
            public const string ClientIsInternal = "ClientIsInternal";
            public const string IsPTOColumn = "IsPTO";
            public const string IsHolidayColumn = "IsHoliday";
            public const string IsAdministrativeColumn = "IsAdministrative";
            public const string AssignedProject = "AssignedProject";
            public const string HolidayDateColumn = "HolidayDate";
            public const string IsORTTimeTypeColumn = "IsORTTimeType";
            public const string ApprovedByColumn = "ApprovedBy";
            public const string ApprovedByNameColumn = "ApprovedByName";
            public const string ApprovedByFirstNameColumn = "ApprovedByFirstName";
            public const string ApprovedByLastNameColumn = "ApprovedByLastName";
            public const string IsORTColumn = "IsORT";
            public const string BillableHours = "BillableHours";
            public const string NonBillableHours = "NonBillableHours";
            public const string GroupByCerteria = "GroupByCerteria";
            public const string Category = "Category";
            public const string UtlizationPercent = "UtlizationPercent";
            public const string ProjectRoleName = "ProjectRoleName";
            public const string ClientCodeColumn = "ClientCode";
            public const string GroupCodeColumn = "GroupCode";
            public const string TimeTypeCodeColumn = "TimeTypeCode";
            public const string BillableHoursUntilToday = "BillableHoursUntilToday";
            public const string ForecastedHoursUntilToday = "ForecastedHoursUntilToday";
            public const string IsPersonNotAssignedToFixedProject = "IsPersonNotAssignedToFixedProject";
            public const string IsFixedProject = "IsFixedProject";
            public const string ProjectNonBillableHours = "ProjectNonBillableHours";
            public const string BusinessDevelopmentHours = "BusinessDevelopmentHours";
            public const string InternalHours = "InternalHours";
            public const string AdminstrativeHours = "AdminstrativeHours";
            public const string TimescaleColumn = "Timescale";
            public const string BillingType = "BillingType";
            public const string EmployeeNumber = "EmployeeNumber";
            public const string BranchID = "BranchID";
            public const string DeptID = "DeptID";
            public const string TotalHours = "TotalHours";
            public const string PTOHours = "PTOHours";
            public const string HolidayHours = "HolidayHours";
            public const string JuryDutyHours = "JuryDutyHours";
            public const string BereavementHours = "BereavementHours";
            public const string ORTHours = "ORTHours";
            public const string UnpaidHours = "UnpaidHours";
            public const string SickOrSafeLeaveHours = "SickOrSafeLeaveHours";
            public const string PaychexID = "PaychexID";
            public const string IsOffshore = "IsOffshore";
            public const string IsNoteRequired = "IsNoteRequired";
            public const string ClientIsNoteRequired = "ClientIsNoteRequired";
            public const string OriginalHours = "OriginalHours";
            public const string Phase = "Phase";
            public const string TimescaleName = "TimescaleName";
            public const string IsSalaryType = "IsSalaryType";
            public const string IsHourlyType = "IsHourlyType";
            public const string ProjectOwnerId = "ProjectOwnerId";
            public const string ProjectOwnerLastName = "ProjectOwnerLastName";
            public const string ProjectOwnerFirstName = "ProjectOwnerFirstName";
            public const string DivisionId = "DivisionId";
            public const string Active = "Active";
            public const string ProjectsCount = "ProjectsCount";
            public const string BusinessUnitId = "BusinessUnitId";
            public const string BusinessUnitName = "BusinessUnitName";
            public const string PersonsCountColumn = "PersonsCount";
            public const string IsUnpaidTimeType = "IsUnpaidTimeType";
            public const string IsUnpaidColoumn = "IsUnpaid";
            public const string SowBudgetColumn = "SowBudget";
            public const string CategoryId = "CategoryId";
            public const string InUseFutureColumn = "InUseFuture";
            public const string PayPersonIdColumn = "PayPersonId";
            public const string VacationDaysColumn = "VacationDays";
            public const string OpportunintyDescription = "OpportunintyDescription";
            public const string ProjectDescription = "ProjectDescription";
            public const string RecruiterIdColumn = "RecruiterId";
            public const string RecruiterFirstNameColumn = "RecruiterFirstName";
            public const string RecruiterLastNameColumn = "RecruiterLastName";
            public const string TerminationReasonIdColumn = "TerminationReasonId";
            public const string TerminationReasonColumn = "TerminationReason";
            public const string ActivePersonsAtTheBeginning = "ActivePersonsAtTheBeginning";
            public const string NewHiredInTheRange = "NewHiredInTheRange";
            public const string TerminationsInTheRange = "TerminationsInTheRange";
            public const string TerminationsW2SalaryCountInTheRange = "TerminationsW2SalaryCountInTheRange";
            public const string TerminationsW2HourlyCountInTheRange = "TerminationsW2HourlyCountInTheRange";
            public const string TerminationsContractorsCountInTheRange = "TerminationsContractorsCountInTheRange";
            public const string SeniorityIdColumn = "SeniorityId";
            public const string Seniority = "Seniority";
            public const string SeniorityCategoryId = "SeniorityCategoryId";
            public const string SeniorityCategory = "SeniorityCategory";
            public const string TerminationsCountInTheRange = "TerminationsCountInTheRange";
            public const string IsW2SalaryRule = "IsW2SalaryRule";
            public const string IsW2HourlyRule = "IsW2HourlyRule";
            public const string Is1099Rule = "Is1099Rule";
            public const string IsContingentRule = "IsContingentRule";
            public const string IsVisible = "IsVisible";
            public const string SeniorityValue = "SeniorityValue";
            public const string Terminations1099HourlyCountInTheRange = "Terminations1099HourlyCountInTheRange";
            public const string Terminations1099PORCountInTheRange = "Terminations1099PORCountInTheRange";
            public const string NewHiredCumulativeInTheRange = "NewHiredCumulativeInTheRange";
            public const string TerminationsCumulativeEmployeeCountInTheRange = "TerminationsCumulativeEmployeeCountInTheRange";
            public const string FirstHireDate = "FirstHireDate";
            public const string LastTerminationDate = "LastTerminationDate";
            public const string IsW2HourlyAllowed = "IsW2HourlyAllowed";
            public const string IsW2SalaryAllowed = "IsW2SalaryAllowed";
            public const string IsSickLeaveColumn = "IsSickLeave";
            public const string CapabilityId = "CapabilityId";
            public const string ProjectCapabilityIds = "ProjectCapabilityIds";
            public const string PracticeAbbreviation = "PracticeAbbreviation";
            public const string Abbreviation = "Abbreviation";
            public const string CapabilityName = "CapabilityName";
            public const string IsJuryDuty = "IsJuryDuty";
            public const string IsBereavement = "IsBereavement";
            public const string IsTimeOffExists = "IsTimeOffExists";
            public const string WeeklyUtlization = "WeeklyUtlization";
            public const string TitleId = "TitleId";
            public const string Title = "Title";
            public const string TitleTypeId = "TitleTypeId";
            public const string TitleType = "TitleType";
            public const string SortOrder = "SortOrder";
            public const string PTOAccrual = "PTOAccrual";
            public const string MinimumSalary = "MinimumSalary";
            public const string MaximumSalary = "MaximumSalary";
        }

        #endregion

        #region Nested type: FunctionNames

        public class FunctionNames
        {
        }

        #endregion

        #region Nested type: ParameterNames

        /// <summary>
        /// Stored procedures parameter names
        /// </summary>
        public class ParameterNames
        {
            public const string GroupIdColumn = "GroupId";
            public const string AttachmentFileName = "@FileName";
            public const string AttachmentData = "@AttachmentData";
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
            public const string TimescaleNamesListParam = "@TimeScaleNamesList";
            public const string ActivePersons = "ActivePersons";
            public const string TerminationDate = "@TerminationDate";
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
            public const string CloneBillingNotes = "CloneBillingNotes";
            public const string CloneCommissions = "CloneCommissions";
            public const string ConsultantsCanAdjust = ColumnNames.ConsultantsCanAdjust;
            public const string CurrentId = "CurrentId";
            public const string IsDiscussionReview2 = "@IsDiscussionReview2";
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
            public const string IsAdministrative = "@IsAdministrative";
            public const string IsInserted = "@IsInserted";
            public const string IsChargeable = ColumnNames.IsChargeable;
            public const string IsStartDateChangeReflectedForMilestoneAndPersons = "@IsStartDateChangeReflectedForMilestoneAndPersons";
            public const string IsEndDateChangeReflectedForMilestoneAndPersons = "@IsEndDateChangeReflectedForMilestoneAndPersons";
            public const string IsMarginColorInfoEnabled = "@IsMarginColorInfoEnabled";
            public const string IsCorrect = ColumnNames.IsCorrect;
            public const string IsProjectChargeable = ColumnNames.IsProjectChargeable;
            public const string IsDefault = ColumnNames.IsDefault;
            public const string IsReviewed = ColumnNames.IsReviewed;
            public const string ReviewStatusId = ColumnNames.ReviewStatusId;
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
            public const string HasTimeEntries = "HasTimeEntries";
            public const string ProjectIdList = ColumnNames.ProjectIdList;
            public const string ProjectIsChargeable = ColumnNames.ProjectIsChargeable;
            public const string ProjectName = ColumnNames.ProjectName;
            public const string ProjectNumber = ColumnNames.ProjectNumberColumn;
            public const string ProjectManagerIdsList = "@ProjectManagerIdsList";
            public const string ProjectStatusId = ColumnNames.ProjectStatusId;
            public const string RequesterId = "RequesterId";
            public const string SortExpression = ColumnNames.SortExpression;
            public const string StartDate = ColumnNames.StartDate;
            public const string TimeEntryId = ColumnNames.TimeEntryId;
            public const string TimeTypeId = ColumnNames.TimeTypeId;
            public const string TimeTypeName = ColumnNames.TimeTypeName;
            public const string ProjectIdParam = "@ProjectId";
            public const string UploadedDateParam = "@UploadedDate";
            public const string MilestoneIdParam = "@MilestoneId";
            public const string PersonIdParam = "@PersonId";
            public const string PersonIdListParam = "@PersonIdList";
            public const string PersonIdsParam = "@PersonIds";
            public const string PersonTypesParam = "@PersonTypes";
            public const string StrawManListParam = "@StrawManList";
            public const string OutSideResourcesParam = "@OutSideResources";
            public const string RelationTypeIdParam = "@RelationTypeId";
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
            public const string ProjectOwnerIdParam = "@ProjectOwner";
            public const string Date = "Date";
            public const string DayOff = "DayOff";
            public const string PracticeManagerIdParam = "@PracticeManagerId";
            public const string DirecterIdParam = "@DirectorId";
            public const string SalespersonIdsParam = "@SalespersonIds";
            public const string OpportunityOwnerIdsParam = "@OpportunityOwnerIds";
            public const string OpportunityGroupIdsParam = "@OpportunityGroupIds";
            public const string SalespersonIdParam = "@SalespersonId";
            public const string PracticeIdsParam = "@PracticeIds";
            public const string TimeScaleIdsParam = "@TimeScaleIds";
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
            public const string PriorityIdParam = "@PriorityId";
            public const string OldPriorityIdParam = "@OldPriorityId";
            public const string UpdatedPriorityIdParam = "@UpdatedPriorityId";
            public const string DeletedPriorityIdParam = "@DeletedPriorityId";
            public const string ProjectedStartDateParam = "@ProjectedStartDate";
            public const string ProjectedEndDateParam = "@ProjectedEndDate";
            public const string CloseDateParam = "@CloseDate";
            public const string DescriptionParam = "@Description";
            public const string DisplayNameParam = "@DisplayName";
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
            public const string OpportunityIdsParam = "@OpportunityIds";
            public const string IdParam = "@Id";
            public const string IsProjectIdParam = "@IsProjectId";
            public const string DaysPrevious = "@DaysPrevious";
            public const string Id = "@Id";
            public const string IsSetParam = "@IsSet";
            public const string IsRecurringHoliday = "@IsRecurringHoliday";
            public const string RecurringHolidayId = "@RecurringHolidayId";
            public const string HolidayDescription = "@HolidayDescription";
            public const string RecurringHolidayDate = "@RecurringHolidayDate";
            public const string ActualHoursParam = "@ActualHours";
            public const string AttachmentIdParam = "@AttachmentId";
            public const string IsFloatingHolidayParam = "@IsFloatingHoliday";
            public const string ShowLostParam = "@ShowLost";
            public const string ShowWonParam = "@ShowWon";
            public const string TextParam = "@Text";
            public const string RichTextParam = "@RichText";
            public const string ShowAll = "@ShowAll";
            public const string isOpportunityDescriptionSelected = "@IsOpportunityDescriptionSelected";
            public const string ProjectTimeTypesParam = "@ProjectTimeTypes";
            public const string IsInternalParam = "@IsInternal";
            public const string CanCreateCustomWorkTypesParam = "@CanCreateCustomWorkTypes";
            public const string TimeEntriesXmlParam = "@TimeEntriesXml";
            public const string DateParam = "@Date";
            public const string IncludePTOAndHolidayParam = "@IncludePTOAndHoliday";
            public const string IsOnlyActiveAndInternal = "@IsOnlyActiveAndInternal";
            public const string TimeEntrySectionIdParam = "@TimeEntrySectionId";
            public const string IsDeleteParam = "@IsDelete";
            public const string IsRecursiveParam = "@IsRecursive";
            public const string IsOnlyActiveParam = "@IsOnlyActive";
            public const string IsOnlyEnternalProjectsParam = "@IsOnlyEnternalProjects";
            public const string IncludePTOParam = "@IncludePTO";
            public const string IncludeHolidayParam = "@IncludeHoliday";
            public const string SubstituteDayDateParam = "@SubstituteDayDate";
            public const string HolidayDateParam = "@HolidayDate";
            public const string ApprovedByParam = "@ApprovedBy";
            public const string PersonStatusIdsParam = "@PersonStatusIds";
            public const string OrderByCerteriaParam = "@OrderByCerteria";
            public const string SeniorityIdsParam = "@SeniorityIds";
            public const string CategoryNamesParam = "@CategoryNames";
            public const string PersonRoleNamesParam = "@PersonRoleNames";
            public const string IsNoteRequiredParam = "@IsNoteRequired";
            public const string IncludePersonsWithNoTimeEntriesParam = "@IncludePersonsWithNoTimeEntries";
            public const string TimeTypeIdsParam = "@TimeTypeIds";
            public const string PersonDivisionIdsParam = "@PersonDivisionIds";
            public const string ProjectStatusIdsParam = "@ProjectStatusIds";
            public const string AccountIdParam = "@AccountId";
            public const string BusinessUnitIdsParam = "@BusinessUnitIds";
            public const string ProjectBillingTypesParam = "@ProjectBillingTypes";
            public const string IncludeUnpaidParam = "@IncludeUnpaid";
            public const string IncludeSickLeaveParam = "@IncludeSickLeave";
            public const string SowBudgetParam = "@SowBudget";
            public const string CategoryIdParam = "@CategoryId";
            public const string LinkParam = "@Link";
            public const string InActiveParam = "@InActive";
            public const string StrawmanIdParam = "@StrawmanId";
            public const string FilterWithTodayPay = "@FilterWithTodayPay";
            public const string PayTypeIdsParam = "@PayTypeIdsParam";
            public const string HireDatesParam = "@HireDates";
            public const string RecruiterIdsParam = "@RecruiterIds";
            public const string TerminationReasonIdsParam = "@TerminationReasonIds";
            public const string TerminationDatesParam = "@TerminationDates";
            public const string RoleNameParam = "@RoleName";
            public const string ProjectCapabilityIds = "@ProjectCapabilityIds";
            public const string CapabilityIdParam = "@CapabilityId";
            public const string Abbreviation = "@Abbreviation";
            public const string TimescaleId = "@TimescaleId";
            public const string TitleId = "@TitleId";
            public const string Title = "@Title";
            public const string TitleTypeId = "@TitleTypeId";
            public const string SortOrder = "@SortOrder";
            public const string PTOAccrual = "@PTOAccrual";
            public const string MinimumSalary = "@MinimumSalary";
            public const string MaximumSalary = "@MaximumSalary";
        }

        #endregion

        #region Nested type: ProcedureNames

        /// <summary>
        /// Stored procedure names
        /// </summary>
        public class ProcedureNames
        {

            #region Nested type: ActivityLog

            public class ActivityLog
            {
                public const string ActivityLogListByPeriodProcedure = "dbo.ActivityLogListByPeriod";
                public const string ActivityLogGetCountProcedure = "dbo.ActivityLogGetCount";
                public const string UserActivityLogInsertProcedure = "dbo.UserActivityLogInsert";
                public const string GetDatabaseVersionFunction = "SELECT dbo.GetDatabaseVersion()";
            }
            #endregion

            #region Nested type: ComputedFinancials

            public class ComputedFinancials
            {
                public const string FinancialsListByProjectPeriod = "dbo.FinancialsListByProjectPeriod";
                public const string FinancialsListByProjectPeriodTotal = "dbo.FinancialsListByProjectPeriodTotal";
                public const string FinancialsGetByProject = "dbo.FinancialsGetByProject";
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
                public const string GetCompanyNameProcedure = "dbo.GetCompanyName";
                public const string GetCompanyLogoDataProcedure = "dbo.GetCompanyLogoData";
                public const string CompanyLogoDataSaveProcedure = "dbo.CompanyLogoDataSave";
                public const string SaveSettingsKeyValuePairsProcedure = "dbo.SaveSettingsKeyValuePairs";
                public const string GetSettingsKeyValuePairsProcedure = "dbo.GetSettingsKeyValuePairsBySettingsType";
                public const string SaveMarginInfoDefaultsProcedure = "dbo.SaveMarginInfoDefaults";
                public const string GetMarginColorInfoDefaultsProcedure = "dbo.GetMarginColorInfoDefaults";
                public const string SavePracticesIsNotesRequiredDetailsProcedure = "dbo.SavePracticesIsNotesRequiredDetails";
                public const string SaveQuickLinksForDashBoardProcedure = "dbo.SaveQuickLinksForDashBoard";
                public const string GetQuickLinksByDashBoardTypeProcedure = "dbo.GetQuickLinksByDashBoard";
                public const string DeleteQuickLinkByIdProcedure = "dbo.DeleteQuickLinkById";
                public const string SaveAnnouncement = "dbo.SaveAnnouncement";
                public const string GetLatestAnnouncement = "dbo.GetLatestAnnouncement";

            }

            #endregion

            #region Nested type: Person

            public class Person
            {
                public const string IsPersonAlreadyHavingStatus = "dbo.IsPersonAlreadyHavingStatus";
                public const string SetNewManager = "dbo.SetNewManager";
                public const string SetDefaultManager = "dbo.PersonSetDefaultManager";
                public const string ListManagersSubordinates = "dbo.ListManagersSubordinates";
                public const string PersonListByCategoryTypeAndPeriod = "[dbo].[PersonListByCategoryTypeAndPeriod]";
                public const string UpdateIsWelcomeEmailSentForPerson = "dbo.UpdateIsWelcomeEmailSentForPerson";
                public const string GetNoteRequiredDetailsForSelectedDateRange = "dbo.GetNoteRequiredDetailsForSelectedDateRange";
                public const string IsPersonHaveActiveStatusDuringThisPeriod = "dbo.IsPersonHaveActiveStatusDuringThisPeriod";
                public const string GetCustomRolePagePermissions = "dbo.GetCustomRolePagePermissions";
                public const string IsHavingCustomRolePermission = "dbo.IsHavingCustomRolePermission";
                public const string SaveCustomRolePagePermissions = "dbo.SaveCustomRolePagePermissions";
                public const string PersonsListHavingActiveStatusDuringThisPeriodProcedure = "dbo.PersonsListHavingActiveStatusDuringThisPeriod";
                public const string GetApprovedByManagerListProcedure = "dbo.GetApprovedByManagerList";
                public const string GetPersonListBySearchKeywordProcedure = "dbo.GetPersonListBySearchKeyword";
                public const string GetAllPayTypesProcedure = "dbo.GetAllPayTypes";
                public const string GetStrawManListAllProcedure = "dbo.GetStrawManListAll";
                public const string GetStrawManListAllShortProcedure = "dbo.GetStrawManListAllShort";
                public const string SaveStrawManProcedure = "dbo.SaveStrawMan";
                public const string DeleteStrawmanProcedure = "dbo.DeleteStrawman";
                public const string SaveStrawManFromExistingProcedure = "dbo.SaveStrawManFromExisting";
                public const string GetStrawmanDetailsByIdWithCurrentPayProcedure = "dbo.GetStrawmanDetailsByIdWithCurrentPay";
                public const string PersonFirstLastNameByIdProcedure = "dbo.PersonFirstLastNameById";
                public const string GetPersonDetailsShortByGivenIdsProcedure = "dbo.GetPersonDetailsShortByGivenIds";
                public const string GetConsultantDemandProcedure = "dbo.GetConsultantDemand";
                public const string ConsultantUtilizationReportProcedure = "dbo.ConsultantUtilizationReport";
                public const string ConsultantUtilizationWeeklyProcedure = "dbo.ConsultantUtilizationWeekly";
                public const string ConsultantUtilizationDailyByPersonProcedure = "dbo.ConsultantUtilizationDailyByPerson";
                public const string PersonInsertProcedure = "dbo.PersonInsert";
                public const string PersonUpdateProcedure = "dbo.PersonUpdate";
                public const string PersonListAllSeniorityFilterProcedure = "dbo.PersonListAllSeniorityFilter";
                public const string PersonListAllShortProcedure = "dbo.PersonListAllShort";
                public const string OwnerListAllShortProcedure = "dbo.OwnerListAllShort";
                public const string PersonListShortByRoleAndStatusProcedure = "dbo.PersonListShortByRoleAndStatus";
                public const string PersonListByStatusListProcedure = "dbo.PersonListAllByStatusList";
                public const string GetPersonListByPersonIdListProcedure = "dbo.GetPersonListByPersonIds";
                public const string PersonListAllForMilestoneProcedure = "dbo.PersonListAllForMilestone";
                public const string PersonListRecruiterProcedure = "dbo.PersonListRecruiter";
                public const string PersonGetByIdProcedure = "dbo.PersonGetById";
                public const string PersonOverheadByPersonProcedure = "dbo.PersonOverheadByPerson";
                public const string PersonOverheadByTimescaleProcedure = "dbo.PersonOverheadByTimescale";
                public const string PersonGetCountByCommaSeparatedIdsListProcedure = "dbo.PersonGetCountByCommaSeparatedIdsList";
                public const string PersonListBenchExpenseProcedure = "dbo.PersonListBenchExpense";
                public const string UpdateLastPasswordChangedDateForPersonProcedure = "dbo.UpdateLastPasswordChangedDateForPerson";
                public const string GetPersonListByPersonIdsAndPayTypeIdsProcedure = "dbo.GetPersonListByPersonIdsAndPayTypeIds";
                public const string MilestonePersonListOverheadByPersonProcedure = "dbo.MilestonePersonListOverheadByPerson";
                public const string PersonListSalespersonProcedure = "dbo.PersonListSalesperson";
                public const string PersonListProjectOwnerProcedure = "dbo.PersonListProjectOwner";
                public const string PersonWorkDaysNumberProcedure = "dbo.PersonWorkDaysNumber";
                public const string PersonGetByAliasProcedure = "dbo.PersonGetByAlias";
                public const string MembershipDeleteProcedure = "dbo.MembershipDelete";
                public const string aspnetUsersDeleteUserProcedure = "dbo.aspnet_Users_DeleteUser";
                public const string aspnetMembershipCreateUserProcedure = "dbo.aspnet_Membership_CreateUser";
                public const string MembershipAliasUpdateProcedure = "dbo.MembershipAliasUpdate";
                public const string PersonOneOffListProcedure = "dbo.PersonOneOffList";
                public const string PersonEnsureIntegrityProcedure = "dbo.PersonEnsureIntegrity";
                public const string PersonGetExcelSetProcedure = "dbo.PersonExcelSet";
                public const string PermissionsGetAllowedClientsProcedure = "dbo.PermissionsGetAllowedClients";
                public const string PermissionsGetAllowedGroupsProcedure = "dbo.PermissionsGetAllowedGroups";
                public const string UserTemporaryCredentialsInsertProcedure = "dbo.UserTemporaryCredentialsInsert";
                public const string GetTemporaryCredentialsByUserNameProcedure = "dbo.GetTemporaryCredentialsByUserName";
                public const string SetNewPasswordForUserProcedure = "dbo.aspnet_Membership_SetPassword";
                public const string DeleteTemporaryCredentialsByUserNameProcedure = "dbo.DeleteTemporaryCredentialsByUserName";
                public const string PermissionsGetAllowedPracticeManagersProcedure = "dbo.PermissionsGetAllowedPracticeManagers";
                public const string PermissionsGetAllowedPracticesProcedure = "dbo.PermissionsGetAllowedPractices";
                public const string PermissionsGetAllowedSalespersonsProcedure = "dbo.PermissionsGetAllowedSalespersons";
                public const string PermissionsGetAllProcedure = "dbo.PermissionsGetAll";
                public const string PermissionsSetAllProcedure = "dbo.PermissionsSetAll";
                public const string PersonMilestoneWithFinancials = "dbo.PersonMilestoneWithFinancials";
                public const string PersonListAllSeniorityFilterWithPayProcedure = "dbo.PersonListAllSeniorityFilterWithCurrentPay";
                public const string PersonListAllSeniorityFilterWithPayByCommaSeparatedIdsListProcedure = "dbo.PersonListAllSeniorityFilterWithCurrentPayByCommaSeparatedIdsList";
                public const string GetPasswordHistoryByUserNameProcedure = "dbo.GetPasswordHistoryByUserName";
                public const string GetStrawmanListShortFilterWithTodayPay = "dbo.GetStrawmanListShortFilterWithTodayPay";
                public const string GetTerminationReasonsList = "dbo.GetTerminationReasonsList";
                public const string GetPersonHireAndTerminationDateById = "dbo.GetPersonHireAndTerminationDateById";
                public const string GetPersonListWithRole = "dbo.GetPersonListWithRole";
                public const string GetPersonEmploymentHistoryById = "dbo.GetPersonEmploymentHistoryById";
                public const string GetPersonAdministrativeTimeTypesInRange = "dbo.GetPersonAdministrativeTimeTypesInRange";
                public const string IsPersonTimeOffExistsInSelectedRangeForOtherthanGivenTimescale = "dbo.IsPersonTimeOffExistsInSelectedRangeForOtherthanGivenTimescale";
                public const string GetWeeklyUtilizationForConsultant = "dbo.GetWeeklyUtilizationForConsultant";
            }

            #endregion

            #region Nested type: TimeEntry

            public class TimeType
            {
                public const string GetAll = "dbo.TimeTypeGetAll";
                public const string GetAllAdministrativeTimeTypes = "dbo.GetAllAdministrativeTimeTypes";
                public const string GetAdministrativeChargeCodeValues = "dbo.GetAdministrativeChargeCodeValues";
                public const string Update = "dbo.TimeTypeUpdate";
                public const string Insert = "dbo.TimeTypeInsert";
                public const string Delete = "dbo.TimeTypeDelete";
            }

            public class TimeEntry
            {
                #region Time entry
                public const string Get = "dbo.PersonTimeEntries";
                public const string ListAll = "dbo.TimeEntriesAll";
                public const string GetCount = "dbo.TimeEntriesGetCount";
                public const string GetTotals = "dbo.TimeEntriesGetTotals";
                public const string PersonTimeEntriesByPeriod = "dbo.PersonTimeEntriesByPeriod";
                public const string GetWorkTypeNameByIdProcedure = "dbo.GetWorkTypeNameById";
                public const string GetWorkTypeByIdProcedure = "dbo.GetWorkTypeById";
                public const string ToggleIsReviewed = "dbo.TimeEntryToggleIsReviewed";
                public const string ToggleIsCorrect = "dbo.TimeEntryToggleIsCorrect";
                public const string ToggleIsChargeable = "dbo.TimeEntryToggleIsChargeable";

                //New sproc time track
                public const string DeleteTimeEntryProcedure = "dbo.DeleteTimeEntry";
                public const string SaveTimeTrackProcedure = "dbo.SaveTimeTrack";
                public const string SetPersonTimeEntryRecursiveSelectionProcedure = "dbo.SetPersonTimeEntryRecursiveSelection";
                public const string SetPersonTimeEntrySelectionProcedure = "dbo.SetPersonTimeEntrySelection";
                public const string GetIsChargeCodeTurnOffByPeriodProcedure = "dbo.GetIsChargeCodeTurnOffByPeriod";


                #endregion

                #region Filters

                public const string TimeEntryAllPersons = "dbo.TimeEntryAllPersons";
                public const string TimeEntryAllMilestones = "dbo.TimeEntryAllMilestones";
                public const string TimeEntryAllMilestonesByClientId = "dbo.TimeEntryAllMilestonesByClientId";
                public const string HasTimeEntriesForMilestoneBetweenOldAndNewDates = "dbo.HasTimeEntriesForMilestoneBetweenOldAndNewDates";

                #endregion

                #region Shared

                public const string ConsultantMilestones = "dbo.ConsultantMilestones";
                public const string CheckPersonTimeEntriesAfterTerminationDate = "dbo.CheckPersonTimeEntriesAfterTerminationDate";
                #endregion

                #region Reports

                public const string TimeEntriesGetByProject = "dbo.TimeEntriesGetByProject";
                public const string TimeEntriesGetByPersonId = "dbo.TimeEntriesGetByPersonId";
                public const string TimeEntriesGetByPersonsForExcel = "dbo.TimeEntriesGetByPersonsForExcel";
                public const string TimeEntriesGetByProjectCumulative = "dbo.TimeEntryHoursByPersonProject";

                #endregion
            }

            #endregion

            #region Nested type: Pay

            public class Pay
            {
                public const string PayGetCurrentByPersonProcedure = "dbo.PayGetCurrentByPerson";
                public const string PayGetHistoryByPersonProcedure = "dbo.PayGetHistoryByPerson";
                public const string GetPayHistoryShortByPersonProcedure = "dbo.GetPayHistoryShortByPerson";
                public const string PaySaveProcedure = "dbo.PaySave";
                public const string PayDeleteProcedure = "dbo.PayDelete";
                public const string IsPersonSalaryTypeListByPeriodProcedure = "dbo.IsPersonSalaryTypeListByPeriod";
            }

            #endregion

            #region Nested type: Seniority

            public class Seniority
            {
                public const string SeniorityListAllProcedure = "dbo.SeniorityListAll";
                public const string ListAllSeniorityCategories = "dbo.ListAllSeniorityCategories";
            }

            #endregion

            #region Nested type: Title

            public class Title
            {
                public const string GetAllTitles = "dbo.GetAllTitles";
                public const string GetTitleById = "dbo.GetTitleById";
                public const string TitleInset= "dbo.TitleInset";
                public const string TitleUpdate= "dbo.TitleUpdate";
                public const string TitleDelete= "dbo.TitleDelete";
            }

            #endregion

            #region Nested type: Practices

            public class Practices
            {
                public const string GetAll = "dbo.PracticeListAll";
                public const string GetById = "dbo.PracticeGetById";
                public const string Update = "dbo.PracticeUpdate";
                public const string Insert = "dbo.PracticeInsert";
                public const string Delete = "dbo.PracticeDelete";
                public const string GetPracticeCapabilities = "dbo.GetPracticeCapabilities";
                public const string PracticeListAllWithCapabilities = "dbo.PracticeListAllWithCapabilities";
                public const string CapabilityDelete = "dbo.CapabilityDelete";
                public const string CapabilityUpdate = "dbo.CapabilityUpdate";
                public const string CapabilityInsert = "dbo.CapabilityInsert";
            }
            #endregion

            #region Nested type: Reports

            public class Reports
            {
                public const string PersonTimeEntriesDetails = "dbo.PersonTimeEntriesDetails";
                public const string PersonTimeEntriesSummary = "dbo.PersonTimeEntriesSummary";
                public const string GetPersonTimeEntriesTotalsByPeriod = "dbo.GetPersonTimeEntriesTotalsByPeriod";
                public const string TimePeriodSummaryReportByResource = "dbo.TimePeriodSummaryReportByResource";
                public const string TimePeriodSummaryReportByProject = "dbo.TimePeriodSummaryReportByProject";
                public const string TimePeriodSummaryReportByWorkType = "dbo.TimePeriodSummaryReportByWorkType";
                public const string ProjectSummaryReportByResource = "dbo.ProjectSummaryReportByResource";
                public const string ProjectDetailReportByResource = "dbo.ProjectDetailReportByResource";
                public const string ProjectSummaryReportByWorkType = "dbo.ProjectSummaryReportByWorkType";
                public const string GetMilestonesForProject = "dbo.GetMilestonesForProject";
                public const string TimePeriodSummaryByResourcePayCheck = "dbo.TimePeriodSummaryByResourcePayCheck";
                public const string TimeEntryAuditReport = "dbo.TimeEntryAuditReport";
                public const string AccountSummaryReportByProject = "dbo.AccountSummaryByProject";
                public const string AccountSummaryReportByBusinessUnit = "dbo.AccountSummaryByBusinessUnit";
                public const string AccountSummaryByBusinessDevelopment = "dbo.AccountSummaryByBusinessDevelopment";
                public const string NewHireReport = "dbo.NewHireReport";
                public const string TerminationReport = "dbo.TerminationReport";
                public const string TerminationReportGraph = "dbo.TerminationReportGraph";
            }
            #endregion

            #region Nested type: Calendar

            public class Calendar
            {
                public const string PersonCalendarGetProcedure = "dbo.PersonCalendarGet";
                public const string CalendarGetProcedure = "dbo.CalendarGet";
                public const string CalendarUpdateProcedure = "dbo.CalendarUpdate";
                public const string SaveTimeOffProcedure = "dbo.SaveTimeOff";
                public const string SaveSubstituteDayProcedure = "dbo.SaveSubstituteDay";
                public const string WorkDaysCompanyNumberProcedure = "dbo.WorkDaysCompanyNumber";
                public const string WorkDaysPersonNumberProcedure = "dbo.WorkDaysPersonNumber";
                public const string GetCompanyHolidaysProcedure = "dbo.GetCompanyHolidays";
                public const string GetRecurringHolidaysList = "dbo.GetRecurringHolidaysList";
                public const string SetRecurringHoliday = "dbo.SetRecurringHoliday";
                public const string GetRecurringHolidaysInWeek = "dbo.GetRecurringHolidaysInWeek";
                public const string DeleteSubstituteDayProcedure = "dbo.DeleteSubstituteDay";
                public const string GetTimeOffSeriesPeriod = "dbo.GetTimeOffSeriesPeriod";
                public const string GetSubstituteDate = "dbo.GetSubstituteDate";
                public const string GetSubstituteDayDetails = "dbo.GetSubstituteDayDetails";
                public const string CalendarGetWithBasicInfo = "dbo.CalendarGetWithBasicInfo";

            }
            #endregion

            #region Nested type: ProjectGroup

            public class ProjectGroup
            {
                public const string ProjectGroupListAll = "dbo.ProjectGroupListAll";
                public const string ProjectGroupRenameByClient = "dbo.ProjectGroupRenameByClient";
                public const string ProjectGroupInsert = "dbo.ProjectGroupInsert";
                public const string ProjectGroupDelete = "dbo.ProjectGroupDelete";
                public const string GetInternalBusinessUnits = "dbo.GetInternalBusinessUnits";
                public const string ListGroupByClientAndPersonInPeriod = "dbo.ListGroupByClientAndPersonInPeriod";
                public const string GetClientsGroups = "dbo.GetClientsGroups";

            }
            #endregion

            #region Nested type: Note

            public class Note
            {
                public const string NoteGetByTargetId = "dbo.NoteGetByTargetId";
                public const string NoteInsert = "dbo.NoteInsert";
                public const string NoteUpdate = "dbo.NoteUpdate";
                public const string NoteDelete = "dbo.NoteDelete";
            }
            #endregion

            #region Nested type: Project

            public class Project
            {
                public const string GetProjectsByClientId = "dbo.GetProjectsByClientId";
                public const string GetProjectListByDateRange = "dbo.GetProjectListByDateRange";
                public const string ProjectListAll = "dbo.ProjectListAll";
                public const string ProjectListAllMultiParameters = "dbo.ProjectListAllMultiParameters";
                public const string ProjectsListByClient = "dbo.ProjectsListByClient";
                public const string ListProjectsByClientShort = "dbo.ListProjectsByClientShort";
                public const string ProjectsListByClientWithSort = "dbo.ProjectsListByClientWithSort";
                public const string ProjectsCountByClient = "dbo.ProjectsCountByClient";
                public const string ProjectGetById = "dbo.ProjectGetById";
                public const string ProjectInsert = "dbo.ProjectInsert";
                public const string ProjectUpdate = "dbo.ProjectUpdate";
                public const string SaveProjectAttachment = "dbo.SaveProjectAttachment";
                public const string DeleteProjectAttachmentByProjectId = "dbo.DeleteProjectAttachmentByProjectId";
                public const string ProjectSetStatus = "dbo.ProjectSetStatus";
                public const string ProjectSearchText = "dbo.ProjectSearchText";
                public const string CloneProject = "dbo.CloneProject";
                public const string ProjectGetByNumber = "dbo.ProjectGetByNumber";
                public const string ProjectShortGetByNumber = "dbo.ProjectShortGetByNumber";
                public const string ProjectMilestonesFinancials = "dbo.ProjectMilestonesFinancials";
                public const string GetProjectListWithFinancials = "dbo.GetProjectListWithFinancials";
                public const string GetProjectListForGroupingPracticeManagers = "dbo.GetProjectListForGroupingPracticeManagers";
                public const string CategoryItemBudgetSave = "dbo.CategoryItemBudgetSave";
                public const string CategoryItemListByCategoryType = "dbo.CategoryItemListByCategoryType";
                public const string CalculateBudgetForCategoryItems = "dbo.CalculateBudgetForCategoryItems";
                public const string CategoryItemsSaveFromXML = "dbo.CategoryItemsSaveFromXML";
                public const string GetProjectAttachmentData = "dbo.GetProjectAttachmentData";
                public const string ProjectDelete = "dbo.ProjectDelete";
                public const string ProjectListAllWithoutFiltering = "dbo.ProjectListAllWithoutFiltering";
                public const string IsUserHasPermissionOnProject = "dbo.IsUserHasPermissionOnProject";
                public const string IsUserIsOwnerOfProject = "dbo.IsUserIsOwnerOfProject";
                public const string GetProjectAttachments = "dbo.GetProjectAttachments";
                public const string ProjectsListByProjectGroupId = "dbo.ProjectsListByProjectGroupId";
                public const string GetBusinessDevelopmentProject = "dbo.GetBusinessDevelopmentProject";
                public const string GetProjectByIdShort = "dbo.GetProjectByIdShort";
                public const string GetIsHourlyRevenueByPeriod = "dbo.GetIsHourlyRevenueByPeriod";
                public const string ListProjectsByClientAndPersonInPeriod = "dbo.ListProjectsByClientAndPersonInPeriod";
                public const string ProjectSearchByName = "dbo.ProjectSearchByName";
                public const string GetTimeTypesInUseDetailsByProjectProcedure = "dbo.GetTimeTypesInUseDetailsByProject";
                public const string GetOwnerProjectsAfterTerminationDateProcedure = "dbo.GetOwnerProjectsAfterTerminationDate";
                public const string GetTimeTypesByProjectIdProcedure = "dbo.GetProjectTimeTypes";
                public const string SetProjectTimeTypesProcedure = "dbo.SetProjectTimeTypes";
                public const string GetUnpaidTimeTypeProcedure = "dbo.GetUnpaidTimeType";
                public const string GetSickLeaveTimeTypeProcedure = "dbo.GetSickLeaveTimeType";
                public const string GetPTOTimeTypeProcedure = "dbo.GetPTOTimeType";
                public const string IsUserIsProjectOwner = "dbo.IsUserIsProjectOwner";
                public const string AttachOpportunityToProject = "dbo.AttachOpportunityToProject";

            }
            #endregion

            #region Nested type: MilestonePerson

            public class MilestonePerson
            {
                public const string ConsultantMilestones = "dbo.ConsultantMilestones";
                public const string MilestonePersonListByProject = "dbo.MilestonePersonListByProject";
                public const string MilestonePersonListByProjectShort = "dbo.MilestonePersonListByProjectShort";
                public const string MilestonePersonListByMilestone = "dbo.MilestonePersonListByMilestone";
                public const string MilestonePersonsByMilestoneForTEByProject = "dbo.MilestonePersonsByMilestoneForTEByProject";
                public const string MilestonePersonInsert = "dbo.MilestonePersonInsert";
                public const string IsPersonAlreadyAddedtoMilestone = "dbo.IsPersonAlreadyAddedtoMilestone";
                public const string MilestonePersonDelete = "dbo.MilestonePersonDelete";
                public const string DeleteMilestonePersonEntry = "dbo.DeleteMilestonePersonEntry";
                public const string MilestonePersonEntryInsert = "dbo.MilestonePersonEntryInsert";
                public const string MilestonePersonDeleteEntries = "dbo.MilestonePersonDeleteEntries";
                public const string MilestonePersonGetByMilestonePersonId = "dbo.MilestonePersonGetByMilestonePersonId";
                public const string MilestonePersonEntryListByMilestonePersonId = "dbo.MilestonePersonEntryListByMilestonePersonId";
                public const string CheckTimeEntriesForMilestonePerson = "dbo.CheckTimeEntriesForMilestonePerson";
                public const string CheckTimeEntriesForMilestonePersonWithGivenRoleId = "dbo.CheckTimeEntriesForMilestonePersonWithGivenRoleId";
                public const string MilestonePersonsGetByMilestoneId = "dbo.MilestonePersonsGetByMilestoneId";
                public const string MilestonePersonEntriesWithFinancialsByMilestoneId = "dbo.MilestonePersonEntriesWithFinancialsByMilestoneId";
                public const string MilestonePersonEntryWithFinancials = "dbo.MilestonePersonEntryWithFinancials";
                public const string UpdateMilestonePersonEntry = "dbo.UpdateMilestonePersonEntry";
                public const string MilestoneResourceUpdateProcedure = "dbo.MilestoneResourceUpdate";
            }
            #endregion

            #region Nested type: ProjectExpenses

            public class ProjectExpenses
            {
                public const string GetById = "dbo.ProjectExpenseGetById";
                public const string GetAllForMilestone = "dbo.ProjectExpenseGetAllForMilestone";
                public const string GetAllForProject = "dbo.ProjectExpenseGetAllForProject";
                public const string Update = "dbo.ProjectExpenseUpdate";
                public const string Insert = "dbo.ProjectExpenseInsert";
                public const string Delete = "dbo.ProjectExpenseDelete";
            }
            #endregion

            #region Nested type: Opportunitites

            public class Opportunitites
            {
                public const string OpportunityTransitionGetByOpportunity = "dbo.OpportunityTransitionGetByOpportunity";
                public const string OpportunityTransitionGetByPerson = "dbo.OpportunityTransitionGetByPerson";
                public const string OpportunityTransitionInsert = "dbo.OpportunityTransitionInsert";
                public const string OpportunityTransitionDelete = "dbo.OpportunityTransitionDelete";
                public const string OpportunityListAll = "dbo.OpportunityListAll";
                public const string FilteredOpportunityListAll = "dbo.FilteredOpportunityListAll";
                public const string OpportunityListWithMinimumDetails = "dbo.OpportunityListWithMinimumDetails";
                public const string OpportunityGetById = "dbo.OpportunityGetById";
                public const string OpportunityInsert = "dbo.OpportunityInsert";
                public const string OpportunityUpdate = "dbo.OpportunityUpdate";
                public const string OpportunityGetExcelSet = "dbo.OpportunityExcelSet";
                public const string OpportunityGetByNumber = "dbo.OpportunityGetByNumber";
                public const string GetOpportunityPersons = "dbo.GetOpportunityPersons";
                public const string ConvertOpportunityToProject = "dbo.ConvertOpportunityToProject";
                public const string OpportunityPersonInsert = "dbo.OpportunityPersonInsert";
                public const string OpportunityPriorityInsert = "dbo.OpportunityPriorityInsert";
                public const string OpportunityPriorityUpdate = "dbo.OpportunityPriorityUpdate";
                public const string OpportunityPriorityDelete = "dbo.OpportunityPriorityDelete";
                public const string UpdatePriorityIdForOpportunity = "dbo.UpdatePriorityIdForOpportunity";
                public const string OpportunityPersonDelete = "dbo.OpportunityPersonDelete";
                public const string OpportunityPrioritiesListAll = "dbo.OpportunityPrioritiesListAll";
                public const string OpportunityPriorities = "dbo.GetOpportunityPriorities";
                public const string GetPersonsByOpportunityIds = "dbo.GetPersonsByOpportunityIds";
                public const string OpportunityDelete = "dbo.OpportunityDelete";
                public const string IsOpportunityPriorityInUse = "dbo.IsOpportunityPriorityInUse";
                public const string IsOpportunityHaveTeamStructure = "dbo.IsOpportunityHaveTeamStructure";
                public const string GetOpportunityPriorityTransitionCount = "dbo.GetOpportunityPriorityTransitionCount";
                public const string GetOpportunityStatusChangeCount = "dbo.GetOpportunityStatusChangeCount";
                public const string OpportunitySearchText = "dbo.OpportunitySearchText";
                public const string AttachProjectToOpportunity = "dbo.AttachProjectToOpportunity";

            }
            #endregion

            #region Nested type: OverHeads

            public class OverHeads
            {
                public const string GetMLFHistory = "dbo.GetMLFHistory";
            }
            #endregion

            #region Nested type: Client

            public class Client
            {
                public const string ClientInsertProcedure = "dbo.ClientInsert";
                public const string ClientUpdateProcedure = "dbo.ClientUpdate";
                public const string ClientListAllProcedure = "dbo.ClientListAll";
                public const string ClientGetByIdProcedure = "dbo.ClientGetById";
                public const string ClientListAllForProjectProcedure = "dbo.ClientListAllForProject";
                public const string UpdateIsChargableForClientProcedure = "dbo.UpdateIsChargableForClient";
                public const string ColorsListAllProcedure = "dbo.ColorsListAll";
                public const string GetClientMarginColorInfoProcedure = "dbo.GetClientMarginColorInfo";
                public const string ClientMarginColorInfoInsertProcedure = "dbo.ClientMarginColorInfoInsert";
                public const string ClientListAllWithoutPermissionsProcedure = "dbo.ClientListAllWithoutPermissions";
                public const string GetInternalAccountProcedure = "dbo.GetInternalAccount";
                public const string UpdateStatusForClient = "dbo.UpdateStatusForClient";
                public const string ClientIsNoteRequired = "dbo.ClientIsNoteRequiredUpdate";
            }

            #endregion

            #region Nested type: Commission

            public class Commission
            {
                public const string CommissionGetByProjectTypeProcedure = "dbo.CommissionGetByProjectType";
                public const string CommissionSetProcedure = "dbo.CommissionSet";
            }
            #endregion

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

