CREATE TABLE [Skills].[Employer](
	[EmployerId] [int] NOT NULL IDENTITY(1,1),
	[Description] [nvarchar](max) NULL,
	[DisplayOrder] [tinyint] NULL,
	[IsActive]		[bit] NOT NULL CONSTRAINT DF_Employer_IsActive DEFAULT(1),
	[IsDeleted]		[bit] NOT NULL CONSTRAINT DF_Employer_IsDeleted DEFAULT(0),
 CONSTRAINT [PK_Employer] PRIMARY KEY CLUSTERED 
(
	[EmployerId] ASC
)
)
