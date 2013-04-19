CREATE TABLE [dbo].[ProjectGroup] (
    [GroupId]			INT				IDENTITY (0, 1) NOT NULL,
    [ClientId]			INT				NOT NULL,
	[Code]				NVARCHAR(5)		NOT NULL,
    [Name]				NVARCHAR (100)	NOT NULL,
	[Active]			BIT				NOT NULL CONSTRAINT DF_ProjectGroup_Active DEFAULT  1,
	[IsInternal]		BIT				NOT NULL CONSTRAINT DF_ProjectGroup_IsInternal DEFAULT 0,
	[BusinessGroupId]	INT				NOT NULL,
    PRIMARY KEY CLUSTERED ([GroupId] ASC) WITH (IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF)
);
