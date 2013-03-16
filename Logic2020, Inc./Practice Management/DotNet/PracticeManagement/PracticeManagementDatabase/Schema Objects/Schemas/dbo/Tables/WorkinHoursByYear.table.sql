CREATE TABLE [dbo].[WorkinHoursByYear]
(
	[Year]					INT	NOT NULL,
	[YearStartDate]			DATE NOT NULL,
	[YearEndDate]			DATE NOT NULL,
	[DaysInYear]			INT NOT NULL,
	[HoursInYear]			INT NOT NULL
	CONSTRAINT PK_WorkinHoursByYear PRIMARY KEY ([Year] ASC,[YearStartDate] ASC, [YearEndDate] ASC),
)

