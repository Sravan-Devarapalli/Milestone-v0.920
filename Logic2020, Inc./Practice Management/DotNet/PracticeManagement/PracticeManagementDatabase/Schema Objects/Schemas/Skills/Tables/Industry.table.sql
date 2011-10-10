CREATE TABLE [Skills].[Industry](
	[IndustryId] [int] NOT NULL IDENTITY(1,1),
	[Description] [nvarchar](max) NULL,
	[DisplayOrder] [tinyint] NULL,
	[IsActive]		[bit] NOT NULL CONSTRAINT DF_Industry_IsActive DEFAULT(1),
	[IsDeleted]		[bit] NOT NULL CONSTRAINT DF_Industry_IsDeleted DEFAULT(0),
 CONSTRAINT [PK_Industry] PRIMARY KEY CLUSTERED 
(
	[IndustryId] ASC
)
)
