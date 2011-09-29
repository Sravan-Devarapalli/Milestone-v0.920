CREATE TABLE [dbo].[PersonEmployer](
	[EmployerId] [int] NOT NULL,
	[PersonId] [bigint] NOT NULL,
	[Title] [nvarchar](100) NOT NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[DisplayOrder] [tinyint] NULL,
	[ModificationDate] [datetime] NOT NULL,
 CONSTRAINT [PK_PersonEmployer] PRIMARY KEY CLUSTERED 
(
	[EmployerId] ASC,
	[PersonId] ASC
)
)
GO
