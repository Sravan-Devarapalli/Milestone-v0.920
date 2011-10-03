CREATE TABLE [Skills].[Employer](
	[EmployerId] [int] NOT NULL IDENTITY(1,1),
	[Description] [nvarchar](max) NULL,
	[DisplayOrder] [tinyint] NULL,
	[TenantId]		[int] NOT NULL,
 CONSTRAINT [PK_Employer] PRIMARY KEY CLUSTERED 
(
	[TenantId] ASC,
	[EmployerId] ASC
)
)
