CREATE TABLE [dbo].[TerminationReasons]
(
	TerminationReasonId	INT IDENTITY(1,1) NOT NULL,
	TerminationReason	NVARCHAR(2000) NOT NULL,
	IsW2SalaryRule		BIT NOT NULL CONSTRAINT DF_TerminationReasons_IsW2SalaryRule DEFAULT(0),
	IsW2HourlyRule		BIT NOT NULL CONSTRAINT DF_TerminationReasons_IsW2HourlyRule DEFAULT(0),
	Is1099Rule			BIT NOT NULL CONSTRAINT DF_TerminationReasons_Is1099Rule DEFAULT(0),
	IsContingentRule	BIT NOT NULL CONSTRAINT DF_TerminationReasons_IsContingentRule DEFAULT(0),
	IsVisible			BIT NOT NULL CONSTRAINT DF_TerminationReasons_IsVisible DEFAULT(1),
	CONSTRAINT PK_TerminationReasons_TerminationReasonId PRIMARY KEY (TerminationReasonId)
)
