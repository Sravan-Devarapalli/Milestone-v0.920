CREATE TABLE [Skills].[TrainingType](
	[TrainingTypeId] [int] NOT NULL IDENTITY(1,1),
	[Description] [nvarchar](max) NULL,
	[DisplayOrder] [tinyint] NULL,
	[IsActive]		[bit] NOT NULL CONSTRAINT DF_TrainingType_IsActive DEFAULT(1),
	[IsDeleted]		[bit] NOT NULL CONSTRAINT DF_TrainingType_IsDeleted DEFAULT(0),
 CONSTRAINT [PK_TrainingType] PRIMARY KEY CLUSTERED 
(
	[TrainingTypeId] ASC
)
)
