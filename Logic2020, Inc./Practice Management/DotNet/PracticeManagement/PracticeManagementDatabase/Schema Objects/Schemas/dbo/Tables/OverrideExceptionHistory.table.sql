CREATE TABLE [dbo].[OverrideExceptionHistory]
(
	[Id]				INT IDENTITY(1,1) NOT NULL,
	[PersonId]			INT         NOT NULL,
	OverrideStartDate	DATETIME	NULL,
	OverrideEndDate		DATETIME	NULL,
	ModifiedDate		DATETIME	NOT NULL,
	ModifiedBy			INT			NULL,
	CONSTRAINT PK_OverrideExceptionHistory PRIMARY KEY (Id)
)

