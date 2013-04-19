CREATE TABLE [dbo].[BusinessGroup]
(
	[BusinessGroupId]	INT				IDENTITY (0, 1) NOT NULL,
    [ClientId]			INT				NOT NULL,
	[Code]				NVARCHAR(6)		NOT NULL,
    [Name]				NVARCHAR (100)	NOT NULL,
	[Active]			BIT				NOT NULL CONSTRAINT DF_BusinessGroup_Active DEFAULT  1,
    PRIMARY KEY CLUSTERED ([BusinessGroupId] ASC) WITH (IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF)
)

