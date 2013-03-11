CREATE TABLE [dbo].[ProjectSummaryCache]
(
	[Id]							INT IDENTITY (1, 1) NOT NULL,
	[ProjectId]						INT NOT NULL,
	[MonthStartDate]				DATETIME NULL,
	[MonthEndDate]					DATETIME NULL,
	ProjectRevenue					DECIMAL (18, 2) NULL,
	ProjectRevenueNet				DECIMAL (18, 2) NULL,
	Cogs							DECIMAL (18, 2) NULL,
	GrossMargin						DECIMAL (18, 2) NULL,
	ProjectedhoursperMonth			DECIMAL (18, 2)  NULL,
	SalesCommission					DECIMAL (18, 2)  NULL,
	PracticeManagementCommission	INT NULL,
	Expense							INT NULL,
	ReimbursedExpense				INT NULL,
	ActualRevenue					DECIMAL (18, 2) NULL,
	ActualGrossMargin				DECIMAL (18, 2) NULL,
	IsMonthlyRecord					BIT NOT NULL,
	CreatedDate						DATETIME,	
	CacheDate						DATE,
	CONSTRAINT PK_ProjectSummaryCache_Id PRIMARY KEY CLUSTERED([Id])
)

