CREATE TABLE [Skills].[TrainingType](
	[TrainingTypeId] [int] NOT NULL IDENTITY(1,1),
	[Description] [nvarchar](max) NULL,
	[DisplayOrder] [tinyint] NULL,
	[TenantId]		[int] NOT NULL,
 CONSTRAINT [PK_TrainingType] PRIMARY KEY CLUSTERED 
(
	[TenantId] ASC,
	[TrainingTypeId] ASC
)
)
