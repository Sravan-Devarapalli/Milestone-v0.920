CREATE TABLE [Skills].[QualificationType](
	[QualificationTypeId]	[int] NOT NULL IDENTITY(1,1), 
	[Description] [nvarchar](max) NULL,
	[DisplayOrder] [tinyint] NULL,
	[IsActive]		[bit] NOT NULL CONSTRAINT DF_QualificationType_IsActive DEFAULT(1),
	[IsDeleted]		[bit] NOT NULL CONSTRAINT DF_QualificationType_IsDeleted DEFAULT(0),
 CONSTRAINT [PK_QualificationType] PRIMARY KEY CLUSTERED 
(
	[QualificationTypeId] ASC
)
)

