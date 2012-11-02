CREATE TABLE [dbo].[TimeType] (
    [TimeTypeId]		INT	IDENTITY (1, 1) NOT NULL,
    [Code]				NVARCHAR(5)			NOT NULL,
    [Name]				VARCHAR (50)		NOT NULL,
    [IsDefault]			BIT					NOT NULL,
    [IsAllowedToEdit]	BIT					NULL,
    [IsInternal]		BIT					NOT NULL,
	[IsActive]			BIT					NOT NULL,
	[IsAdministrative]  BIT					NOT NULL CONSTRAINT [DF_TimeType_IsAdministrative] DEFAULT 0,
	[IsW2SalaryAllowed] BIT					NOT NULL CONSTRAINT [DF_TimeType_IsW2SalaryAllowed] DEFAULT 0,
	[IsW2HourlyAllowed] BIT					NOT NULL CONSTRAINT [DF_TimeType_IsW2HourlyAllowed] DEFAULT 0
);
 

