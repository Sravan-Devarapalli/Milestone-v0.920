CREATE TABLE [dbo].[PersonStatus] (
    [PersonStatusId] INT           NOT NULL,
    [Name]           NVARCHAR (25) NULL,
	[IsPersonStatus]	BIT NOT NULL CONSTRAINT DF_PersonStatus_IsPersonStatus DEFAULT(1)
);
