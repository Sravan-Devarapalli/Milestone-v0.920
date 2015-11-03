CREATE TABLE [dbo].[Practice] (
    [PracticeId]        INT           IDENTITY (1, 1) NOT NULL,
    [Name]              VARCHAR (100) NOT NULL,
    [PracticeManagerId] INT           NULL,
    [IsActive]          BIT           NOT NULL,
    [IsCompanyInternal] BIT           NOT NULL,
	[Abbreviation]      NVARCHAR (100)  NULL,
	IsNotesRequired     BIT   CONSTRAINT Df_Practice_IsNotesRequired DEFAULT (1) NOT NULL,
	[ShowInUtilizationReport]	BIT NOT NULL CONSTRAINT DF_Practice_ShowInUtilizationReport DEFAULT(1)
);


