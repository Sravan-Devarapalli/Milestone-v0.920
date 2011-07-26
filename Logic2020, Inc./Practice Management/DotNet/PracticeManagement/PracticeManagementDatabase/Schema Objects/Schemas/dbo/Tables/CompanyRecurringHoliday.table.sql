CREATE TABLE [dbo].[CompanyRecurringHoliday](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Description] [nvarchar](255) NOT NULL,
	[IsSet] [bit] NOT NULL,
	[Month] [int] NOT NULL,
	[Day] [int] NULL,
	[NumberInMonth] [int] NULL,
	[DayOfTheWeek] [int] NULL,
	 CONSTRAINT [PK_CompanyRecurringHoliday] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
)
