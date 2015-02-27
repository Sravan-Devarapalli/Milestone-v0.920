CREATE TABLE [dbo].[BlockHistory]
(
	[Id]				INT IDENTITY(1,1) NOT NULL,
	[PersonId]			INT         NOT NULL,
	BlockStartDate		DATETIME	NULL,
	BlockEndDate		DATETIME	NULL,
	ModifiedDate		DATETIME	NOT NULL,
	ModifiedBy			INT			NULL,
	CONSTRAINT PK_BlockHistory PRIMARY KEY (Id)
)

