CREATE TABLE [dbo].[ManagedParametersByPerson]
(
	PersonId				INT				NOT NULL,
	ActualRevenuePerHour	DECIMAL(32,2)	NULL,
	TargetRevenuePerHour	DECIMAL(32,2)	NULL,
	HoursUtilization		DECIMAL(32,2)	NULL,
	TargetRevenuePerAnnum	DECIMAL(32,2)	NULL,
	CONSTRAINT PK_ManagedParametersByPerson PRIMARY KEY (PersonId)
)

