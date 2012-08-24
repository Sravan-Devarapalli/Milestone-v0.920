CREATE TABLE [dbo].[TerminationReasons]
(
	TerminationReasonId	INT IDENTITY(1,1),
	TerminationReason	NVARCHAR(2000),
	IsW2SalaryRule		BIT CONSTRAINT DF_TerminationReasons_IsW2SalaryRule DEFAULT(0),
	IsW2HourlyRule		BIT CONSTRAINT DF_TerminationReasons_IsW2HourlyRule DEFAULT(0),
	Is1099Rule			BIT CONSTRAINT DF_TerminationReasons_Is1099Rule DEFAULT(0),
	IsContingentRule	BIT CONSTRAINT DF_TerminationReasons_IsContingentRule DEFAULT(0),
	CONSTRAINT PK_TerminationReasons_TerminationReasonId PRIMARY KEY (TerminationReasonId)
)
