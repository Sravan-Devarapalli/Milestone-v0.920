CREATE TABLE [Skills].[PersonEmployer](
	[EmployerId] [int] NOT NULL,
	[PersonId] [int] NOT NULL,
	[Title] [nvarchar](100) NOT NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[DisplayOrder] [tinyint] NULL,
	[ModifiedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_PersonEmployer] PRIMARY KEY CLUSTERED 
(
	[EmployerId] ASC,
	[PersonId] ASC
)
)
GO
