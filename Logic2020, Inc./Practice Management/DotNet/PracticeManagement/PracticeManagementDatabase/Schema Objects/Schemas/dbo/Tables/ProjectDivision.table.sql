CREATE TABLE [dbo].[ProjectDivision]
(
	[DivisionId]    INT             IDENTITY (1, 1) NOT NULL,
	[DivisionName]  NVARCHAR (50)   NOT NULL,
	[IsExternal]    BIT             NOT NULL
	CONSTRAINT [PK_ProjectDivision_DivisionId]	PRIMARY KEY (DivisionId)
)

