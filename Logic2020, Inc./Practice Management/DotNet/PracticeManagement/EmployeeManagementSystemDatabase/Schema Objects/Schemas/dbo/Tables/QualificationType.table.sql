CREATE TABLE [dbo].[QualificationType](
	[QualificationTypeId]	[int] NOT NULL IDENTITY(1,1), 
	[Description] [nvarchar](max) NULL,
	[DisplayOrder] [tinyint] NULL,
 CONSTRAINT [PK_QualificationType] PRIMARY KEY CLUSTERED 
(
	[QualificationTypeId] ASC
)
)

