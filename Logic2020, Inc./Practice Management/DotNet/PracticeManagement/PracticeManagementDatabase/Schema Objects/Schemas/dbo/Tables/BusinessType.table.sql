CREATE TABLE [dbo].[BusinessType]
(
	[BusinessTypeId]       INT            NOT NULL,
	[Name]                   NVARCHAR (100)  NULL,       
	PRIMARY KEY CLUSTERED ([BusinessTypeId] ASC) WITH (IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF)
)

