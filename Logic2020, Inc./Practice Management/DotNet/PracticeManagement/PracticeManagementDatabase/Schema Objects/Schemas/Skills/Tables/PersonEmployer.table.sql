CREATE TABLE [Skills].[PersonEmployer](
	[EmployerId] [int] NOT NULL,
	[PersonId] [bigint] NOT NULL,
	[Title] [nvarchar](100) NOT NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[DisplayOrder] [tinyint] NULL,
	[ModificationDate] [datetime] NOT NULL,
	[TenantId]		[int] NOT NULL,
 CONSTRAINT [PK_PersonEmployer] PRIMARY KEY CLUSTERED 
(
	[TenantId] ASC,
	[EmployerId] ASC,
	[PersonId] ASC
)
)
GO
