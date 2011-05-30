CREATE TABLE [dbo].[MinimumLoadFactorHistoryForUI](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NULL,
	[W2-Hourly] [decimal](18, 5) NOT NULL,
	[W2-Salary] [decimal](18, 5) NOT NULL,
	[1099] [decimal](18, 5) NOT NULL,
CONSTRAINT PK_MinimumLoadFactorHistoryForUI_Id PRIMARY KEY CLUSTERED(Id)
)

