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
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
