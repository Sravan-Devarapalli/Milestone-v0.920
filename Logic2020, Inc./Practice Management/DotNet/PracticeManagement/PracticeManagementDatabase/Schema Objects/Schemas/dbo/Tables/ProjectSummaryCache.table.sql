CREATE TABLE [dbo].[ProjectSummaryCache]
(
	[Id]							INT IDENTITY (1, 1) NOT NULL,
	[ProjectId]						INT NOT NULL,
	[MonthStartDate]				DATETIME NULL,
	[MonthEndDate]					DATETIME NULL,
	[RangeType]						NVARCHAR(20) NULL,
	ProjectRevenue					DECIMAL (18, 2) NULL,
	ProjectRevenueNet				DECIMAL (18, 2) NULL,
	Cogs							DECIMAL (18, 2) NULL,
	GrossMargin						DECIMAL (18, 2) NULL,
	ProjectedhoursperMonth			DECIMAL (18, 2)  NULL,
	Expense							INT NULL,
	ReimbursedExpense				INT NULL,
	ActualRevenue					DECIMAL (18, 2) NULL,
	ActualGrossMargin				DECIMAL (18, 2) NULL,
	PreviousMonthActualRevenue		DECIMAL (18, 2) NULL,
	PreviousMonthActualGrossMargin	DECIMAL (18, 2) NULL,
	IsMonthlyRecord					BIT NOT NULL,
	CreatedDate						DATETIME,	
	CacheDate						DATE,
	CONSTRAINT PK_ProjectSummaryCache_Id PRIMARY KEY CLUSTERED([Id])
)

